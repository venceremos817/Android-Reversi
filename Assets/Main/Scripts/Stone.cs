using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stone : MonoBehaviour
{
    public enum StoneColor
    {
        Black,
        White,

        MAX
    }

    [SerializeField]
    private RectTransform _rectTransform = null;
    [SerializeField]
    private Image _image = null;
    [SerializeField]
    private Animator _anim = null;
    private StoneColor _color;
    public StoneColor color { get { return _color; } }

    private void Awake()
    {
        if (_rectTransform == null)
        {
            _rectTransform = GetComponent<RectTransform>();
        }
        if (_image == null)
        {
            _image = GetComponent<Image>();
        }
        if (_anim == null)
        {
            _anim = GetComponent<Animator>();
        }
    }

    static public Stone Place(Cell cell, StoneColor color)
    {
        var ret = Instantiate(GameBoard.Instance.Stone, cell.transform);
        switch (color)
        {
            case StoneColor.Black:
                ret._image.color = Color.black;
                break;
            case StoneColor.White:
                ret._image.color = Color.white;
                break;
        }
        ret._color = color;
        return ret;
    }

    public void Reverse()
    {
        _color = (StoneColor)(((int)_color + 1) % (int)StoneColor.MAX);

        // êFêÿÇËë÷Ç¶
        switch (_color)
        {
            case StoneColor.Black:
                _image.color = Color.black;
                break;
            case StoneColor.White:
                _image.color = Color.white;
                break;
            default:
                _image.color = Color.red;
                break;
        }
        _anim.Play("Rotation", 0, 0.0f);
    }
}
