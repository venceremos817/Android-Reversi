using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public struct ID
    {
        public ID(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public int x, y;
    }
    public enum State
    {
        None,
        Black,
        White
    }
    [SerializeField]
    private RectTransform _rectTransform = null;
    public ID _id;
    private State _state;




    public RectTransform rectTransform { get => _rectTransform; }
    public ID id { get => _id; set => _id = value; }
    public State state { get => _state; set => _state = value; }
}
