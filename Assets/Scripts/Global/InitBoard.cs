using UnityEngine;

namespace Global
{
    public class InitBoard : MonoBehaviour
    {
        [SerializeField] private GameObject mid;
        [SerializeField] private GameObject side;
        [SerializeField] private GameObject corner;

        private readonly GameObject[,] _board = new GameObject[19, 19];

        private void Start()
        {
            for (var x = -8; x <= 8; x++)
            for (var y = -8; y <= 8; y++)
            {
                _board[x + 9, y + 9] = Instantiate(mid, new Vector3(x * 0.5f, y * 0.5f, 0), Quaternion.identity);
                _board[x + 9, y + 9].transform.SetParent(gameObject.transform);
            }

            for (var i = -8; i <= 8; i++)
            {
                _board[i + 9, 0] = Instantiate(side, new Vector3(i * 0.5f, 4.5f, 0), Quaternion.Euler(0, 0, -90));
                _board[i + 9, 18] = Instantiate(side, new Vector3(i * 0.5f, -4.5f, 0), Quaternion.Euler(0, 0, 90));
                _board[0, i + 9] = Instantiate(side, new Vector3(-4.5f, i * 0.5f, 0), Quaternion.Euler(0, 0, 0));
                _board[18, i + 9] = Instantiate(side, new Vector3(4.5f, i * 0.5f, 0), Quaternion.Euler(0, 0, 180));
                _board[i + 9, 0].transform.SetParent(gameObject.transform);
                _board[i + 9, 18].transform.SetParent(gameObject.transform);
                _board[0, i + 9].transform.SetParent(gameObject.transform);
                _board[18, i + 9].transform.SetParent(gameObject.transform);
            }

            _board[0, 0] = Instantiate(corner, new Vector3(-4.5f, 4.5f, 0), Quaternion.Euler(0, 0, 0));
            _board[18, 0] = Instantiate(corner, new Vector3(4.5f, 4.5f, 0), Quaternion.Euler(0, 0, -90));
            _board[0, 18] = Instantiate(corner, new Vector3(-4.5f, -4.5f, 0), Quaternion.Euler(0, 0, 90));
            _board[18, 18] = Instantiate(corner, new Vector3(4.5f, -4.5f, 0), Quaternion.Euler(0, 0, 180));
            _board[0, 0].transform.SetParent(gameObject.transform);
            _board[18, 0].transform.SetParent(gameObject.transform);
            _board[0, 18].transform.SetParent(gameObject.transform);
            _board[18, 18].transform.SetParent(gameObject.transform);
        }
    }
}