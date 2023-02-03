using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// �`�F�V���L�̕\����R���g���[������R���|�[�l���g
/// </summary>
public class CheshireCatFaceController : MonoBehaviour
{
    #region serialize
    [Header("Variables")]
    [SerializeField]
    float _blinkSwitchTime = 0.05f;

    [SerializeField]
    float _broadlySwitchTime = 0.05f;

    [SerializeField]
    float _talkSwitchTime = 0.05f;

    [Tooltip("���̂܂΂������J�n����܂ł̑ҋ@���Ԃ̍ő�l")]
    [SerializeField]
    float _blinkMaxInterval = 5.0f;

    [Tooltip("���݂̕\��̎��")]
    [SerializeField]
    CheshireCatFaceType _currentFaceType = default;

    [Header("Components")]
    [Tooltip("�`�F�V���L��Renderer")]
    [SerializeField]
    Renderer _catRenderer = default;
    #endregion

    #region private
    Dictionary<CatFacePatternType, Vector2> _catFacePatternDic = new Dictionary<CatFacePatternType, Vector2>();
    Coroutine _currentFaceCoroutine;
    Material _faceMat;
    bool _init;
    bool _isBlinking = false;
    bool _isGrinning = false;
    bool _isTalking = false;
    #endregion

    #region public
    #endregion

    #region property
    #endregion

    private void OnValidate()
    {
        if (_init)
        {
            ChangeFaceType(_currentFaceType);
        }
    }
    private void Awake()
    {
        int faceCount = 0;

        for (int i = 0; i < 4; i++)
        {
            for (int n = 0; n < 2; n++)
            {
                _catFacePatternDic.Add((CatFacePatternType)faceCount, new Vector2(n * 0.5f, i * -0.25f));
                faceCount++;
            }
        }
        _faceMat = _catRenderer.materials[1];
        _init = true;
    }

    private void Start()
    {
        EventManager.ListenEvents(Events.Cheshire_Talk, OnTaking);
        EventManager.ListenEvents(Events.Cheshire_StartGrinning, OnGrinning);
        EventManager.ListenEvents(Events.FinishTalking, OnReturnDefaultFace);
        EventManager.ListenEvents(Events.BossStage_FrontCheshire, OnTaking);
    }

    /// <summary>
    /// ��̕\���ύX����
    /// </summary>
    /// <param name="type"> �\��̎�� </param>
    public void ChangeFaceType(CheshireCatFaceType type)
    {
        if (_currentFaceCoroutine != null)
        {
            StopCoroutine(_currentFaceCoroutine);
            _currentFaceCoroutine = null;
            _isBlinking = false;
            _isTalking = false;
        }

        switch (type)
        {
            case CheshireCatFaceType.Default:
                ChangeFacePatternType(CatFacePatternType.Default);
                break;
            case CheshireCatFaceType.Blink:
                _isBlinking = true;
                _currentFaceCoroutine = StartCoroutine(BlinkCoroutine());
                break;
            case CheshireCatFaceType.StartGrinning:
                _isGrinning = true;
                _currentFaceCoroutine = StartCoroutine(StartGrinningCoroutine());
                break;
            case CheshireCatFaceType.EndGrinning:
                _isGrinning = false;
                _currentFaceCoroutine = StartCoroutine(EndGrinningCoroutine());
                break;
            case CheshireCatFaceType.Talking:
                _isTalking = true;
                _currentFaceCoroutine = StartCoroutine(TalkCoroutine());
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// ��̃p�^�[����ύX����
    /// </summary>
    /// <param name="type"> �p�^�[���̎�� </param>
    void ChangeFacePatternType(CatFacePatternType type)
    {
        _faceMat.SetTextureOffset("_MainTex", GetFaceOffset(type));
        _catRenderer.materials[1] = _faceMat;
    }

    /// <summary>
    /// �\��̈ʒu���擾����
    /// </summary>
    /// <param name="type"> �\��̎�� </param>
    /// <returns></returns>
    Vector2 GetFaceOffset(CatFacePatternType type)
    {
        var v = _catFacePatternDic.FirstOrDefault(x => x.Key == type).Value;

        return v;
    }

    /// <summary>
    /// �܂΂����̃R���[�`��
    /// </summary>
    IEnumerator BlinkCoroutine()
    {
        float timer;
        float rand;

        ChangeFacePatternType(CatFacePatternType.Default);

        //�u���̏����̃��[�v
        while (_isBlinking)
        {
            yield return new WaitForSeconds(_blinkSwitchTime);

            ChangeFacePatternType(CatFacePatternType.HalfOpenEye);

            yield return new WaitForSeconds(_blinkSwitchTime);

            ChangeFacePatternType(CatFacePatternType.CloseEye);

            yield return new WaitForSeconds(_blinkSwitchTime);

            ChangeFacePatternType(CatFacePatternType.HalfOpenEye);

            yield return new WaitForSeconds(_blinkSwitchTime);

            ChangeFacePatternType(CatFacePatternType.Default);

            //�����Ŏw�肵���u���̕b���̊ԁA������ҋ@
            timer = 0;
            rand = Random.Range(3, _blinkMaxInterval);
            while (_isBlinking && timer < rand)
            {
                timer += Time.deltaTime;
                yield return null;
            }
        }
    }

    /// <summary>
    /// �j������ɂȂ�R���[�`��
    /// </summary>
    IEnumerator StartGrinningCoroutine()
    {
        _isGrinning = true;

        ChangeFacePatternType(CatFacePatternType.Default);

        yield return new WaitForSeconds(_broadlySwitchTime);

        ChangeFacePatternType(CatFacePatternType.Broadly_First);

        yield return new WaitForSeconds(_broadlySwitchTime);

        ChangeFacePatternType(CatFacePatternType.Broadly_Second);

        yield return new WaitForSeconds(_broadlySwitchTime);

        ChangeFacePatternType(CatFacePatternType.Broadly_Third);
    }

    /// <summary>
    /// �j�����炩��ʏ��ɖ߂�R���[�`��
    /// </summary>
    IEnumerator EndGrinningCoroutine()
    {
        ChangeFacePatternType(CatFacePatternType.Broadly_Third);

        yield return new WaitForSeconds(_broadlySwitchTime);

        ChangeFacePatternType(CatFacePatternType.Broadly_Second);

        yield return new WaitForSeconds(_broadlySwitchTime);

        ChangeFacePatternType(CatFacePatternType.Broadly_First);

        yield return new WaitForSeconds(_broadlySwitchTime);

        ChangeFacePatternType(CatFacePatternType.Default);
    }

    /// <summary>
    /// ����R���[�`��
    /// </summary>
    IEnumerator TalkCoroutine()
    {
        int random;

        while (_isTalking)
        {
            random = Random.Range(0, 3);

            if (random == 0)
            {
                ChangeFacePatternType(CatFacePatternType.Default);
            }
            else if (random == 1)
            {
                ChangeFacePatternType(CatFacePatternType.Talk1);
            }
            else
            {
                ChangeFacePatternType(CatFacePatternType.Talk2);
            }

            yield return new WaitForSeconds(_talkSwitchTime);
        }
    }

    #region MessageActions
    /// <summary>
    /// ����
    /// </summary>
    void OnTaking()
    {
        ChangeFaceType(CheshireCatFaceType.Talking);
    }
    /// <summary>
    /// �j������
    /// </summary>
    void OnGrinning()
    {
        ChangeFaceType(CheshireCatFaceType.StartGrinning);
    }
    /// <summary>
    /// �ʏ��ɖ߂�
    /// </summary>
    void OnReturnDefaultFace()
    {
        //�b���Ă��Ȃ����͏������s��Ȃ�
        if (!_isTalking)
        {
            return;
        }
        ChangeFaceType(CheshireCatFaceType.Blink);
    }
    #endregion
}

/// <summary>
/// �`�F�V���L�̕\��̎��
/// </summary>
public enum CheshireCatFaceType
{
    /// <summary> �ʏ�̕\�� </summary>
    Default,
    /// <summary> �܂΂��� </summary>
    Blink,
    /// <summary> �ʏ�̊炩��j������ɂȂ� </summary>
    StartGrinning,
    /// <summary> �j�����炩��ʏ��ɂȂ� </summary>
    EndGrinning,
    /// <summary> ���� </summary>
    Talking
}

/// <summary>
/// �`�F�V���L�̕\��p�^�[��
/// </summary>
public enum CatFacePatternType
{
    Default,
    HalfOpenEye,
    CloseEye,
    Broadly_First,
    Broadly_Second,
    Broadly_Third,
    Talk1,
    Talk2
}
