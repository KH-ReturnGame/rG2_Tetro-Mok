using SinglePlay2.State;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.PlayerLoop;

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

        
        public override void Initialize()
        {
            status = AgentStatus.Ready;
        }
        public override void OnEpisodeBegin()
        {
            status = AgentStatus.Ready;
        }
        
        public override void CollectObservations(VectorSensor sensor)
        {
            // 1) 보드 상태 (19×19), 0=빈, 1=흑, 2=백
            int[,] board = _manager.GameBoard;
            for (int x = 0; x < 19; x++)
            for (int y = 0; y < 19; y++)
                sensor.AddObservation(board[x, y]);

            // 2) 현재 놓일 도형 (최소 바운딩 박스)
            int[,] shape = _manager.CurrentShape;
            int w = _manager.ShapeWidth;
            int h = _manager.ShapeHeight;
            for (int i = 0; i < w; i++)
            for (int j = 0; j < h; j++)
                sensor.AddObservation(shape[i, j]);
            int[] cord = { _manager.minX, _manager.maxX, _manager.minY, _manager.maxY };
            foreach (var var in cord)
            {
                sensor.AddObservation(var);
            }

            // 3) 자기 점수 정보
            sensor.AddObservation((AgentType == Type.Black)?_manager.BlackScore:_manager.WhiteScore);
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
            // DiscreteActions[0] 하나만 사용하는 설정이라고 가정
            var discreteActionsOut = actionsOut.DiscreteActions;

            // ↑ ↓ ← → 방향키
            if      (Input.GetKeyDown(KeyCode.UpArrow))    discreteActionsOut[0] = 0;
            else if (Input.GetKeyDown(KeyCode.DownArrow))  discreteActionsOut[0] = 1;
            else if (Input.GetKeyDown(KeyCode.LeftArrow))  discreteActionsOut[0] = 2;
            else if (Input.GetKeyDown(KeyCode.RightArrow)) discreteActionsOut[0] = 3;
            // 회전
            else if (Input.GetKeyDown(KeyCode.R))          discreteActionsOut[0] = 4;
            // 확인(“ok”)
            else if (Input.GetKeyDown(KeyCode.Return))     discreteActionsOut[0] = 5;
        }
    }
}