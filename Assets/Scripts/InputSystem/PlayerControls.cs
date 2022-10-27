//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.3.0
//     from Assets/Scripts/InputSystem/PlayerControls.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PlayerControls : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""fb391da8-7258-4ddb-9d57-4f97937cdac8"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""3b910c82-6264-4d11-b4b1-9b8e738dcc88"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""6c0dd135-1c23-411f-9206-e25abfcf62b9"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""28a346da-1fbd-40b2-bf66-864daf714f50"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""4cb9ecdb-dcc5-4a9f-b3db-2f4c265e7812"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""0dd38373-7e43-4a1c-bddc-7f53322727df"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""1575b8bf-b706-4cf5-9f6e-cbf3e664896b"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""96e2c8a8-c0d6-49d2-94c9-3742c9f159f6"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        },
        {
            ""name"": ""Stage1"",
            ""id"": ""e29ea919-df65-43e6-85bf-c6becfde67bf"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""836a399c-91c5-403c-979d-7b17e07d62e1"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""10b977ff-17b6-436b-82a8-6a66a55368b6"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""7010f2cc-9234-4a2f-9626-b29d80b70430"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""2ba105d4-432f-4a92-8b6c-c011ae20194a"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""637759ec-01fb-4808-94bf-673706e2116e"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""964a67e7-8a56-40ef-ab79-63d064415cf1"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""134002c3-bdb2-4742-9797-1c4bd34220fd"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        },
        {
            ""name"": ""Stage3"",
            ""id"": ""14910024-ca5c-4732-84f4-38130f8ef1ba"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""463fd3e6-4898-4749-8574-6e4068a43731"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Throw"",
                    ""type"": ""Button"",
                    ""id"": ""bdc5057c-fb7f-4fd9-bbae-b727f5a45f8e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ToLeft"",
                    ""type"": ""Button"",
                    ""id"": ""8d95b1ab-cb21-4291-af2c-21b68933d666"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ToRight"",
                    ""type"": ""Button"",
                    ""id"": ""e26ba21a-13ae-468d-8df0-43b93297f8b0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""68880c16-50f1-45f8-8d61-084dd5038f02"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""e8b42287-fb11-4d49-939a-de60b0309a5c"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""5d6552c4-e5fd-40fe-a07e-bd7f380d8c23"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""71ab5a4d-33bc-48af-b642-b098fee57812"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""0e49a530-b026-4f18-857e-ab4f11b6cdc4"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""4cb2a25d-1cbe-445a-a8d4-b843d7d66f16"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ed230e35-5a4d-47c1-9217-67382d74262c"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Throw"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b5610213-abab-4dbf-8cc9-cb44dba6c205"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Throw"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2805029a-c1dd-4977-920c-6300c14ba335"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ToLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""028181bc-a09b-490d-87fa-3f4b8dfb70fa"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ToLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""39193a92-1e6b-4fd7-86f5-ced82f39c67e"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ToRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c9638ebd-501c-460e-9a34-a3926733e938"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ToRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Move = m_Player.FindAction("Move", throwIfNotFound: true);
        // Stage1
        m_Stage1 = asset.FindActionMap("Stage1", throwIfNotFound: true);
        m_Stage1_Move = m_Stage1.FindAction("Move", throwIfNotFound: true);
        // Stage3
        m_Stage3 = asset.FindActionMap("Stage3", throwIfNotFound: true);
        m_Stage3_Move = m_Stage3.FindAction("Move", throwIfNotFound: true);
        m_Stage3_Throw = m_Stage3.FindAction("Throw", throwIfNotFound: true);
        m_Stage3_ToLeft = m_Stage3.FindAction("ToLeft", throwIfNotFound: true);
        m_Stage3_ToRight = m_Stage3.FindAction("ToRight", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_Move;
    public struct PlayerActions
    {
        private @PlayerControls m_Wrapper;
        public PlayerActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Player_Move;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);

    // Stage1
    private readonly InputActionMap m_Stage1;
    private IStage1Actions m_Stage1ActionsCallbackInterface;
    private readonly InputAction m_Stage1_Move;
    public struct Stage1Actions
    {
        private @PlayerControls m_Wrapper;
        public Stage1Actions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Stage1_Move;
        public InputActionMap Get() { return m_Wrapper.m_Stage1; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(Stage1Actions set) { return set.Get(); }
        public void SetCallbacks(IStage1Actions instance)
        {
            if (m_Wrapper.m_Stage1ActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_Stage1ActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_Stage1ActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_Stage1ActionsCallbackInterface.OnMove;
            }
            m_Wrapper.m_Stage1ActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
            }
        }
    }
    public Stage1Actions @Stage1 => new Stage1Actions(this);

    // Stage3
    private readonly InputActionMap m_Stage3;
    private IStage3Actions m_Stage3ActionsCallbackInterface;
    private readonly InputAction m_Stage3_Move;
    private readonly InputAction m_Stage3_Throw;
    private readonly InputAction m_Stage3_ToLeft;
    private readonly InputAction m_Stage3_ToRight;
    public struct Stage3Actions
    {
        private @PlayerControls m_Wrapper;
        public Stage3Actions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Stage3_Move;
        public InputAction @Throw => m_Wrapper.m_Stage3_Throw;
        public InputAction @ToLeft => m_Wrapper.m_Stage3_ToLeft;
        public InputAction @ToRight => m_Wrapper.m_Stage3_ToRight;
        public InputActionMap Get() { return m_Wrapper.m_Stage3; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(Stage3Actions set) { return set.Get(); }
        public void SetCallbacks(IStage3Actions instance)
        {
            if (m_Wrapper.m_Stage3ActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_Stage3ActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_Stage3ActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_Stage3ActionsCallbackInterface.OnMove;
                @Throw.started -= m_Wrapper.m_Stage3ActionsCallbackInterface.OnThrow;
                @Throw.performed -= m_Wrapper.m_Stage3ActionsCallbackInterface.OnThrow;
                @Throw.canceled -= m_Wrapper.m_Stage3ActionsCallbackInterface.OnThrow;
                @ToLeft.started -= m_Wrapper.m_Stage3ActionsCallbackInterface.OnToLeft;
                @ToLeft.performed -= m_Wrapper.m_Stage3ActionsCallbackInterface.OnToLeft;
                @ToLeft.canceled -= m_Wrapper.m_Stage3ActionsCallbackInterface.OnToLeft;
                @ToRight.started -= m_Wrapper.m_Stage3ActionsCallbackInterface.OnToRight;
                @ToRight.performed -= m_Wrapper.m_Stage3ActionsCallbackInterface.OnToRight;
                @ToRight.canceled -= m_Wrapper.m_Stage3ActionsCallbackInterface.OnToRight;
            }
            m_Wrapper.m_Stage3ActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Throw.started += instance.OnThrow;
                @Throw.performed += instance.OnThrow;
                @Throw.canceled += instance.OnThrow;
                @ToLeft.started += instance.OnToLeft;
                @ToLeft.performed += instance.OnToLeft;
                @ToLeft.canceled += instance.OnToLeft;
                @ToRight.started += instance.OnToRight;
                @ToRight.performed += instance.OnToRight;
                @ToRight.canceled += instance.OnToRight;
            }
        }
    }
    public Stage3Actions @Stage3 => new Stage3Actions(this);
    public interface IPlayerActions
    {
        void OnMove(InputAction.CallbackContext context);
    }
    public interface IStage1Actions
    {
        void OnMove(InputAction.CallbackContext context);
    }
    public interface IStage3Actions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnThrow(InputAction.CallbackContext context);
        void OnToLeft(InputAction.CallbackContext context);
        void OnToRight(InputAction.CallbackContext context);
    }
}
