using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AliceProject;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;

public class ShuffleGameManager : StageGame<ShuffleGameManager>
{
    #region serialize
    [Header("Variables")]
    [SerializeField]
    int _phaseCount = 3;

    [Tooltip("��Փx���̃p�����[�^�[")]
    [SerializeField]
    ShuffleGameParameter[] _gameParameters = default;

    [Header("UI")]
    [SerializeField]
    Text _infoText = default;

    [SerializeField]
    Image[] _infoImages = default;

    [SerializeField]
    CanvasGroup _hpGroup = default;

    [SerializeField]
    GameObject[] _juggeInfo = default;

    [SerializeField]
    GameObject _nextInfo = default;

    [Header("Components")]
    [SerializeField]
    Stage2CameraManager _stage2Cameras = default;

    [SerializeField]
    TeacupManager _teacupManager = default;

    [SerializeField]
    TeacupController _teacupCtrl = default;

    [SerializeField]
    PhaseInfo _phaseInfo = default;

    [Header("Debug")]
    [SerializeField]
    bool _debugMode = false;
    #endregion

    #region private
    bool _result = false;
    int _selectIndex;
    #endregion

    #region public
    public override event Action GameSetUp;
    public override event Action GameStart;
    public override event Action GamePause;
    public override event Action GameEnd;
    #endregion

    #region property
    public static new ShuffleGameManager Instance { get; private set; }
    #endregion

    protected override void Awake()
    {
        Instance = this;
    }

    protected override void Start()
    {
        base.Start();
        OnGameStart();
        Init();

        //��肪�N�������p�Ƀ~�j�Q�[�����X�L�b�v����@�\��ǉ�
        this.UpdateAsObservable()
            .Where(_ => UIInput.Next)
            .Take(1)
            .Subscribe(_ =>
            {
                GameManager.SaveStageResult(true);
                TransitionManager.SceneTransition(SceneType.Lobby);
            });
    }

    public override void OnGameSetUp()
    {
    }

    public override void OnGameStart()
    {
        if (!_debugMode)
        {
            StartCoroutine(GameStartCoroutine());
        }
        else
        {
            StartCoroutine(InGameCoroutine());
        }
    }

    public override void OnGameEnd()
    {
    }

    /// <summary>
    /// �Q�[���J�n���̃R���[�`��
    /// </summary>
    /// <param name="action"> �R���[�`���I������Action </param>
    protected override IEnumerator GameStartCoroutine(Action action = null)
    {
        LetterboxController.ActivateLetterbox(true, 0f);
        yield return _stage2Cameras.StartDirectionCoroutine();

        yield return new WaitForSeconds(0.5f);

        _stage2Cameras.ChangeCamera(Stage2CameraType.CloseupMouse);

        yield return new WaitForSeconds(2.5f);

        _teacupManager.AllCupDown();

        yield return new WaitForSeconds(1.1f);

        _teacupManager.OnMouseAnimation(MouseState.CloseEar);

        yield return new WaitForSeconds(1.4f);

        LetterboxController.ActivateLetterbox(false, 1.5f);
        _stage2Cameras.ChangeCamera(Stage2CameraType.Ingame);

        yield return new WaitForSeconds(2.5f);

        //_infoText.text = "�X�^�[�g�I";
        _infoImages[0].enabled = true;

        yield return new WaitForSeconds(2.0f);

        //_infoText.text = "";
        _infoImages[0].enabled = false;

        StartCoroutine(InGameCoroutine());
    }

    IEnumerator InGameCoroutine()
    {
#if UNITY_EDITOR
        if (_debugMode)
        {
            yield return new WaitForSeconds(1.5f);
        }
#endif

        for (int i = 0; i < _phaseCount; i++)
        {
            _result = false;
            _selectIndex = -1;
            _hpGroup.alpha = 1;

            yield return _phaseInfo.OnAnimation(i);

            yield return new WaitForSeconds(1.0f);

            //�V���b�t���J�n�B�I������܂őҋ@
            yield return _teacupCtrl.ShuffleCoroutine((ShufflePhase)i, _teacupManager.Teacups);

            //�e�t�F�C�Y
            switch (i)
            {
                case 0:
                    yield return MessagePlayer.Instance.PlayMessageCorountine(MessageType.FirstVisit_Stage2_Phase1);
                    break;
                case 1:
                    yield return MessagePlayer.Instance.PlayMessageCorountine(MessageType.FirstVisit_Stage2_Phase2);
                    break;
                case 2:
                    yield return MessagePlayer.Instance.PlayMessageCorountine(MessageType.FirstVisit_Stage2_Phase3);
                    break;
                default:
                    break;
            }

            yield return null;

            //�J�b�v��I������܂őҋ@�B�I�������J�b�v�̃f�[�^��Callback�Ŏ擾
            yield return _teacupManager.ChoicePhaseCoroutine((judge, index) =>
            {
                _result = judge;
                _selectIndex = index;
            });

            yield return new WaitUntil(() => _selectIndex > -1);

            if (_result)
            {
                _teacupManager.SelectCupOpen(_selectIndex);
                AudioManager.PlaySE(SEType.UI_CursolMove);
                _stage2Cameras.ChangeCamera(Stage2CameraType.CloseupMouse);

                yield return new WaitForSeconds(0.1f);

                _teacupManager.OnMouseAnimation(MouseState.WakeUp, 0.1f);

                yield return new WaitForSeconds(2.4f);

                //�����Ő�����UI��\��
                //_infoText.text = "�����I";
                _juggeInfo[0].SetActive(true);
                AudioManager.PlaySE(SEType.Stage2_Correct); //�������Đ�

                yield return new WaitForSeconds(1.5f);

                _nextInfo.SetActive(true);

                yield return new WaitUntil(() => UIInput.Submit);
            }
            else
            {
                _teacupManager.SelectCupOpen(_selectIndex);
                AudioManager.PlaySE(SEType.UI_CursolMove);

                yield return new WaitForSeconds(1.5f);

                _stage2Cameras.ChangeCamera(Stage2CameraType.Main, 1);

                yield return new WaitForSeconds(1.0f);

                _teacupManager.AllCupOpen();

                yield return new WaitForSeconds(0.4f);

                _teacupManager.OnMouseAnimation(MouseState.OpenEar, 0.2f);

                yield return new WaitForSeconds(1.6f);

                _juggeInfo[1].SetActive(true);
                HPManager.Instance.ChangeHPValue(1);
                AudioManager.PlaySE(SEType.Stage2_Wrong); //�s�������Đ�

                yield return new WaitForSeconds(1.5f);

                //�̗͂��u0�v�ɂȂ�����Q�[���I�[�o�[���o
                if (HPManager.Instance.CurrentHP.Value <= 0)
                {
                    GameoverDirection.Instance.OnGameoverDirection();
                    yield break;
                }
                _nextInfo.SetActive(true);

                yield return new WaitUntil(() => UIInput.Submit);

                _stage2Cameras.ChangeCamera(Stage2CameraType.CloseupMouse);
                _juggeInfo[1].SetActive(false);
                _nextInfo.SetActive(false);

                yield return new WaitForSeconds(2.5f);

                //���s�����ꍇ�͂�����x�����t�F�C�Y���J�n
                i--;
            }

            if (i < _phaseCount - 1)
            {
                _infoText.text = "";
                _juggeInfo[0].SetActive(false);
                _juggeInfo[1].SetActive(false);
                _nextInfo.SetActive(false);

                _teacupManager.AllCupDown();

                yield return new WaitForSeconds(1.1f);

                _teacupManager.OnMouseAnimation(MouseState.CloseEar, 0.1f);

                yield return new WaitForSeconds(0.9f);

                _stage2Cameras.ChangeCamera(Stage2CameraType.Ingame);

                yield return new WaitForSeconds(2.5f);
            }
            else
            {
                _juggeInfo[0].SetActive(false);
                _juggeInfo[1].SetActive(false);
                _nextInfo.SetActive(false);
                //_infoText.text = "�X�e�[�W�N���A�I";
                _infoImages[1].enabled = true;
                _hpGroup.alpha = 0;

                AudioManager.PlayBGM(BGMType.ClearJingle, false);

                yield return new WaitForSeconds(3.0f);

                //_infoText.text = "";
                _infoImages[1].enabled = false;

                if (!GameManager.CheckStageStatus())
                {
                    yield return GameManager.GetStillDirectionCoroutine(Stages.Stage2, MessageType.GetStill_Stage2);
                }
                else
                {
                    GameManager.ChangeLobbyState(LobbyState.Default);
                }

                GameManager.SaveStageResult(true);
                TransitionManager.FadeIn(FadeType.Black_TransParent, 0f);
                TransitionManager.SceneTransition(SceneType.Lobby);
            }
        }
    }

    protected override IEnumerator GameEndCoroutine(Action action = null)
    {
        throw new NotImplementedException();
    }

    protected override void Init()
    {
        TransitionManager.FadeOut(FadeType.Normal);
        AudioManager.PlayBGM(BGMType.Stage2);
        HPManager.Instance.RecoveryHP();

        _teacupManager.RandomHideMouse();
        _infoImages[0].enabled = false;
        _infoImages[1].enabled = false;
        _infoText.text = "";
        _hpGroup.alpha = 0;
        _juggeInfo[0].SetActive(false);
        _juggeInfo[1].SetActive(false);
        _nextInfo.SetActive(false);

        //���݂̃Q�[���̓�Փx���擾���Đ��l�𔽉f
        var paramIndex = (int)GameManager.Instance.CurrentGameDifficultyType;
        _teacupCtrl.SetParameter(_gameParameters[paramIndex].Parameters);
    }
}
/// <summary>
/// �V���b�t���Q�[���̃t�F�C�Y�̎��
/// </summary>
public enum ShuffleGamePhase
{
    Shuffle_ExChange,
    Shuffle_ExChangeAndWarp,
    Shuffle_High_ExChangeAndWarp
}

/// <summary>
/// �V���b�t���Q�[���̊e��Փx���̃p�����[�^�[
/// </summary>
[Serializable]
public struct ShuffleGameParameter
{
    public string ParamName;
    public DifficultyType DifficultyType;
    public ShuffleParameter[] Parameters;
}
