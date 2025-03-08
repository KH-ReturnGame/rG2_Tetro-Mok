using UnityEngine;

public class InitBoard : MonoBehaviour
{
    [SerializeField] private GameObject mid;
    [SerializeField] private GameObject side;
    [SerializeField] private GameObject corner;

    GameObject[,] _board = new GameObject[19, 19];

    void Start()
    {
        for (int x = -8; x <= 8; x++)
        {
            for (int y = -8; y <= 8; y++)
            {
                _board[x+8, y+8] = Instantiate(mid, new Vector3(x * 0.5f, y * 0.5f, 0), Quaternion.identity);
            }
        }

        for (int i = -8; i <= 8; i++)
        {
            _board[i+8, 0] = Instantiate(side, new Vector3(i * 0.5f, 4.5f, 0), Quaternion.Euler(0, 0, 90));
            _board[i+8, 18] = Instantiate(side, new Vector3(i * 0.5f, -4.5f, 0), Quaternion.Euler(0, 0, -90));
            _board[0, i+8] = Instantiate(side, new Vector3(-4.5f, i * 0.5f, 0), Quaternion.Euler(0, 0, 180));
            _board[18, i+8] = Instantiate(side, new Vector3(4.5f, i * 0.5f, 0), Quaternion.Euler(0, 0, 0));
        }

        _board[0,0] = Instantiate(corner, new Vector3(-4.5f, 4.5f, 0), Quaternion.Euler(0, 0, 180));
        _board[18,0] = Instantiate(corner, new Vector3(4.5f, 4.5f, 0), Quaternion.Euler(0, 0, 90));
        _board[18,18] = Instantiate(corner, new Vector3(4.5f, -4.5f, 0), Quaternion.identity);
        _board[0,18] = Instantiate(corner, new Vector3(-4.5f, -4.5f, 0), Quaternion.Euler(0, 0, -90));
    }
}