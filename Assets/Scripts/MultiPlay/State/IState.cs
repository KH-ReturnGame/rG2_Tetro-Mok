namespace MultiPlay.State
{
    public interface IState
    {
        void OnEnter();
        void OnExit();
        void Update();
        void HandleInput(string input);
    }

    internal enum MoveDirection
    {
        Up,
        Down,
        Left,
        Right,
        Rotate,
        Idle
    }
}