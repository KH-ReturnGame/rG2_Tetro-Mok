using UnityEngine;
using static GlobalGameData;

public class PrevStones : MonoBehaviour
{
    [SerializeField] private GameObject privateBlackStone;
    [SerializeField] private GameObject privateWhiteStone;
    private static GameObject _blackStone;
    private static GameObject _whiteStone;


    void Start()
    {
        _blackStone = privateBlackStone;
        _whiteStone = privateWhiteStone;
    }

    public static void LocateStones(int[,] currentStones)
    {
        for (int i = 0; i < 19; i++)
        for (int j = 0; j < 19; j++)
            if (currentStones[i, j] == 1)
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
                    stone = Instantiate(_whiteStone, new Vector3((i-9)*0.5f, (j-9)*0.5f, 0), Quaternion.identity);
                }

                stone.transform.SetParent(GameObject.Find("PrevStones").transform);
            }
    }
}
