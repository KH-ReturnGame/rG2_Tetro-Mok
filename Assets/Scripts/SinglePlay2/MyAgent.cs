using SinglePlay2.State;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.Collections.Generic;
using UnityEngine;

namespace SinglePlay2
{
    public enum Type
    {
        Black,
        White
    }
    public class MyAgent: Agent
    {
        public Type AgentType;
        public GameManager _manager;
        public AgentStatus status;

        private bool hb = false;
        
        public override void Initialize()
        {
            base.Initialize();
            status = AgentStatus.Ready;
        }

        public override void OnEpisodeBegin()
        {
            status = AgentStatus.Ready;
    
            // 현재 상태가 자신의 차례에 해당하는 상태인지 확인하고,
            // 자신의 차례라면 ReadyToChoose로 변경
            if ((AgentType == Type.Black && (_manager._currentState is BlackState || _manager._currentState is InitialBlackState)) ||
                (AgentType == Type.White && _manager._currentState is WhiteState))
            {
                status = AgentStatus.ReadyToChoose;
            }
        }
        
        public override void CollectObservations(VectorSensor sensor)
        {
            // 기존 관찰 코드 유지
            // 1) 보드 상태 (19×19)를 더 효과적으로 표현
            int[,] board = _manager.GameBoard;

            // 자신의 돌과 상대방 돌을 분리하여 인식하도록 함
            for (int x = 0; x < 19; x++)
            for (int y = 0; y < 19; y++)
            {
                // 내 돌인 경우 1, 아닌 경우 0
                if (AgentType == Type.Black)
                    sensor.AddObservation(board[x, y] == 1 ? 1 : 0);
                else
                    sensor.AddObservation(board[x, y] == 2 ? 1 : 0);

                // 상대 돌인 경우 1, 아닌 경우 0
                if (AgentType == Type.Black)
                    sensor.AddObservation(board[x, y] == 2 ? 1 : 0);
                else
                    sensor.AddObservation(board[x, y] == 1 ? 1 : 0);

                // 보너스 돌인 경우 1, 아닌 경우 0
                sensor.AddObservation(board[x, y] == 3 ? 1 : 0);
            }

            // 2) 현재 조각 위치 정보를 더 명확하게 표현
            int[,] shape = _manager.CurrentShape;
            int w = _manager.ShapeWidth;
            int h = _manager.ShapeHeight;

            // 현재 모양의 중심점 계산 (x, y 좌표의 평균)
            float centerX = 0, centerY = 0;
            int count = 0;
            for (int i = 0; i < w; i++)
            for (int j = 0; j < h; j++)
                if (shape[i, j] != 0)
                {
                    centerX += _manager.minX + i;
                    centerY += _manager.minY + j;
                    count++;
                }
            centerX /= count;
            centerY /= count;

            // 현재 모양의 중심점 상대 위치 (보드 중앙으로부터)
            sensor.AddObservation((centerX - 9) / 9);// -1 ~ 1 범위 정규화
            sensor.AddObservation((centerY - 9) / 9);// -1 ~ 1 범위 정규화
            
            // 3) 게임 상태 정보 추가
            sensor.AddObservation(_manager.BlackScore);
            sensor.AddObservation(_manager.WhiteScore);
            sensor.AddObservation(_manager.count / (float)_manager.max_count_);// 게임 진행도

            // 4) 자신이 현재 플레이 중인 턴인지 여부 (1 또는 0)
            bool isMyTurn = (AgentType == Type.Black &&
                             (_manager._currentState is BlackState || _manager._currentState is InitialBlackState)) ||
                            (AgentType == Type.White && _manager._currentState is WhiteState);
            sensor.AddObservation(isMyTurn ? 1 : 0);
        }
        
        // 행동 마스크 설정 함수 추가
        public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
        {
            // 현재 턴이 자신의 차례가 아니라면 모든 행동 비활성화
            bool isMyTurn = (AgentType == Type.Black && 
                            (_manager._currentState is BlackState || _manager._currentState is InitialBlackState)) ||
                           (AgentType == Type.White && _manager._currentState is WhiteState);
            
            if (!isMyTurn)
            {
                // 모든 행동 비활성화
                for (int i = 0; i < 6; i++)
                {
                    actionMask.SetActionEnabled(0, i, false);
                }
                return;
            }
            
            // 현재 상태에 따라 마스크 적용
            if (_manager._currentState is BlackState blackState)
            {
                ApplyMaskForState(blackState, actionMask);
            }
            else if (_manager._currentState is WhiteState whiteState)
            {
                ApplyMaskForState(whiteState, actionMask);
            }
            else if (_manager._currentState is InitialBlackState initialBlackState)
            {
                ApplyMaskForState(initialBlackState, actionMask);
            }
        }
        
        private void ApplyMaskForState(IState state, IDiscreteActionMask actionMask)
        {
            // 현재 조각의 위치 정보 가져오기
            int minX = _manager.minX;
            int maxX = _manager.maxX;
            int minY = _manager.minY;
            int maxY = _manager.maxY;
            
            // 보드 경계 체크 및 마스킹
            bool canMoveUp = maxY < 18;
            bool canMoveDown = minY > 0;
            bool canMoveLeft = minX > 0;
            bool canMoveRight = maxX < 18;
            
            // 상하좌우 이동 제한
            actionMask.SetActionEnabled(0, 0, canMoveUp);    // Up
            actionMask.SetActionEnabled(0, 1, canMoveDown);  // Down
            actionMask.SetActionEnabled(0, 2, canMoveLeft);  // Left
            actionMask.SetActionEnabled(0, 3, canMoveRight); // Right
            
            // 회전은 항상 가능 (회전 후 경계 체크는 게임로직에서 처리)
            // actionMask.SetActionEnabled(0, 4, true);      // Rotate
            
            // 둘 수 없는 위치에서는 OK 제한
            bool canPlace = CanPlaceCurrentShape();
            actionMask.SetActionEnabled(0, 5, canPlace);     // OK
        }
        
        // 현재 모양을 현재 위치에 놓을 수 있는지 확인
        private bool CanPlaceCurrentShape()
        {
            // 현재 상태에서 _canLocate 값 가져오기
            bool canLocate = true;
            
            if (_manager._currentState is BlackState blackState)
            {
                // 비공개 필드이므로 GameManager 함수를 통해 상태 확인
                canLocate = _manager.CanPlaceCurrentPiece();
            }
            else if (_manager._currentState is WhiteState whiteState)
            {
                canLocate = _manager.CanPlaceCurrentPiece();
            }
            else if (_manager._currentState is InitialBlackState initialBlackState)
            {
                canLocate = _manager.CanPlaceCurrentPiece();
            }
            
            return canLocate;
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            status = AgentStatus.Working;
            var action = actions.DiscreteActions[0];

            switch (action)
            {
                case 0:
                    _manager._currentState.HandleInput("up");
                    break;
                case 1:
                    _manager._currentState.HandleInput("down");
                    break;
                case 2:
                    _manager._currentState.HandleInput("left");
                    break;
                case 3:
                    _manager._currentState.HandleInput("right");
                    break;
                case 4:
                    _manager._currentState.HandleInput("rotate");
                    break;
                case 5:
                    _manager._currentState.HandleInput("ok");
                    break;
            }
        }
        
        public override void Heuristic(in ActionBuffers actionsOut)
        {
            // 휴리스틱 함수는 그대로 유지
            var discreteActionsOut = actionsOut.DiscreteActions;

            hb = false;
            // ↑ ↓ ← → 방향키
            if      (Input.GetKey(KeyCode.W))    discreteActionsOut[0] = 0;
            else if (Input.GetKey(KeyCode.S))  discreteActionsOut[0] = 1;
            else if (Input.GetKey(KeyCode.A))  discreteActionsOut[0] = 2;
            else if (Input.GetKey(KeyCode.D)) discreteActionsOut[0] = 3;
            // 회전
            else if (Input.GetKey(KeyCode.R))          discreteActionsOut[0] = 4;
            // 확인("ok")
            else if (Input.GetKey(KeyCode.Return))     discreteActionsOut[0] = 5;
        }
    }
}