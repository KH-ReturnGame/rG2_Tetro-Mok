using UnityEngine;
using static GlobalGameData;

public class PrevStones : MonoBehaviour
{
    private static GameObject _blackStone;
    private static GameObject _whiteStone;
    [SerializeField] private GameObject privateBlackStone;
    [SerializeField] private GameObject privateWhiteStone;


    private void Start()
    {
        _blackStone = privateBlackStone;
        _whiteStone = privateWhiteStone;
    }

    /// <summary>
    ///     배치할 좌표에 따라 GameObject를 생성한다
    /// </summary>
    /// <param name="currentStones">이번 턴에 배치할 돌들의 좌표를 담은 정수 튜플의 리스트</param>
    public static void LocateStones((int, int)[] currentStones)
    {
        foreach (var (i, j) in currentStones)
        {
            GameObject stone;
            if (CurrentState == GameState.BlackTurn)
            {
                MainBoard[i, j] = 1;
                stone = Instantiate(_blackStone, new Vector3((i - 9) * 0.5f, (j - 9) * 0.5f, 0),
                    Quaternion.identity);
            }
            else
            {
                MainBoard[i, j] = 2;
                stone = Instantiate(_whiteStone, new Vector3((i - 9) * 0.5f, (j - 9) * 0.5f, 0),
                    Quaternion.identity);
            }

            stone.name = i + "_" + j;
            stone.transform.SetParent(GameObject.Find("PrevStones").transform);
        }
    }
}