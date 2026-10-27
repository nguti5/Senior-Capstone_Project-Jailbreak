using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    public CreatureBase _base;
    public int _level;
    public bool _isPlayer;
    public Creature Creature{get;set;}

    private Vector3 initalPosition;

    private Image creatureImage;

    private Vector3 hitShakeValue = new Vector3(6.0f,0,0);
    
    private Color initalColor;
    [SerializeField] float startTimeAnimation, attackTimeAnimation, dieTimeAnimation, hitTimeAnimation;
    private void Awake()
    {
        
        creatureImage = GetComponent<Image>();
        initalPosition = creatureImage.transform.localPosition;
        initalColor = creatureImage.color;

    }

    public void SetupCreature()
    {
        Creature = new Creature(_base, _level);
        playStartAnimation();
    }

    public void playStartAnimation()
    {
        if (_isPlayer)
        {
            creatureImage.transform.localPosition = new Vector3(initalPosition.x - 400, initalPosition.y);
        }
        else if (!_isPlayer)
        {
            creatureImage.transform.localPosition = new Vector3(initalPosition.x + 400, initalPosition.y);
        }

        creatureImage.transform.DOLocalMoveX(initalPosition.x, startTimeAnimation);
    }

    public void playAttackAnimation()
    {
        var seq = DOTween.Sequence();
        seq.Append(creatureImage.transform.DOLocalMoveX(initalPosition.x+(_isPlayer?1:-1)*30, 0.2f));
        seq.Append(creatureImage.transform.DOLocalMoveX(initalPosition.x,0.3f));
    }

    public void playRecieveAttackAnimation()
    {
        creatureImage.transform.DOShakePosition(1, hitShakeValue, 10, 90, true, true);
        var seq = DOTween.Sequence();
        for (int i = 0; i < 3; i++)
        {
            seq.Append(creatureImage.DOColor(Color.grey, hitTimeAnimation));
            seq.Append(creatureImage.DOColor(initalColor, hitTimeAnimation));
        }
    }

    public void playDeathAnimation()
    {
        var seq = DOTween.Sequence();
        seq.Join(creatureImage.DOFade(0,dieTimeAnimation));
        seq.Join(creatureImage.transform.DOShakePosition(1,6,10,90, false));
    }
}
