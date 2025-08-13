// using UnityEngine;
// using Fusion;
// using TMPro;
// using UnityEngine.UI;
// using System.Collections;

// public class PlayerChat : NetworkBehaviour
// {
//     [SerializeField] private TextMeshProUGUI textMessage1;
//     [SerializeField] private TextMeshProUGUI textMessage2;
//     [SerializeField] private TMP_InputField inputFieldMessage;
//     [SerializeField] private RawImage chatBackground;
//     public Button btnOpenChat;

//     private static PlayerChat localPlayerChat;
//     bool isChatOpen = false;

//     [SerializeField] private NetworkBehaviour movementScript;

//     public override void Spawned()
//     {
//         // Chỉ gán cho local player
//         if (Object.HasInputAuthority)
//         {
//             localPlayerChat = this;

//             textMessage1 = GameObject.Find("tmp_TextChat1").GetComponent<TextMeshProUGUI>();
//             textMessage2 = GameObject.Find("tmp_TextChat2").GetComponent<TextMeshProUGUI>();
//             inputFieldMessage = GameObject.Find("inputF_TextChat").GetComponent<TMP_InputField>();
//             btnOpenChat = GameObject.Find("btn_Chat").GetComponent<Button>();
//             chatBackground = GameObject.Find("img_BackgroundChat").GetComponent<RawImage>();

//             btnOpenChat.onClick.AddListener(OpenChat);

//             inputFieldMessage.gameObject.SetActive(false);
//             chatBackground.gameObject.SetActive(false);
//             textMessage2.gameObject.SetActive(false);
//         }
//     }

//     private void OpenChat()
//     {
//         if (!Object.HasInputAuthority) return;

//         isChatOpen = !isChatOpen;

//         inputFieldMessage.gameObject.SetActive(isChatOpen);
//         chatBackground.gameObject.SetActive(isChatOpen);
//         textMessage1.gameObject.SetActive(isChatOpen);
//         Debug.Log("click chat: " + isChatOpen);

        
//     }

//     public void SendMessageChat()
//     {
//         if (!Object.HasInputAuthority) return;

//         var message = inputFieldMessage.text;
//         if (string.IsNullOrWhiteSpace(message)) return;

//         var id = Runner.LocalPlayer.PlayerId;
//         var text = $"s{id}: {message}";

//         RpcChat(text);
//         inputFieldMessage.text = "";
//         Debug.Log($"Player {id} sent message: {text}");
//     }

//     [Rpc(RpcSources.All, RpcTargets.All)]
//     public void RpcChat(string message)
//     {
//         // Hiển thị lên UI của local player duy nhất
//         if (localPlayerChat != null)
//         {
//             localPlayerChat.DisplayMessage(message);
//         }
//     }

//     public void DisplayMessage(string message)
//     {
//         textMessage2.gameObject.SetActive(true);
//         textMessage1.text += message + "\n";
//         textMessage2.text = message;
//         StartCoroutine(HideTextMessageAfterDelay(5f));
//     }

//     private IEnumerator HideTextMessageAfterDelay(float time)
//     {
//         yield return new WaitForSeconds(time);
//         textMessage2.gameObject.SetActive(false);
//     }

//     private void Update()
//     {
//         if (!Object.HasInputAuthority) return;

//         if (Input.GetKeyDown(KeyCode.Return) && isChatOpen)
//         {
//             SendMessageChat();
//         }

//         if (isChatOpen && inputFieldMessage != null)
//         {
//             inputFieldMessage.ActivateInputField();
//         }
//     }
// }


using UnityEngine;
using Fusion;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class PlayerChat : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI textMessage1;
    [SerializeField] private TextMeshProUGUI textMessage2;
    [SerializeField] private TMP_InputField inputFieldMessage;
    [SerializeField] private RawImage chatBackground;
    public Button btnOpenChat;

    private static PlayerChat localPlayerChat;
    bool isChatOpen = false;

    [SerializeField] private NetworkBehaviour movementScript; // script di chuyển

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            localPlayerChat = this;

            textMessage1 = GameObject.Find("tmp_TextChat1").GetComponent<TextMeshProUGUI>();
            textMessage2 = GameObject.Find("tmp_TextChat2").GetComponent<TextMeshProUGUI>();
            inputFieldMessage = GameObject.Find("inputF_TextChat").GetComponent<TMP_InputField>();
            btnOpenChat = GameObject.Find("btn_Chat").GetComponent<Button>();
            chatBackground = GameObject.Find("img_BackgroundChat").GetComponent<RawImage>();

            btnOpenChat.onClick.AddListener(OpenChat);

            inputFieldMessage.gameObject.SetActive(false);
            chatBackground.gameObject.SetActive(false);
            textMessage2.gameObject.SetActive(false);
        }
    }

    private void OpenChat()
    {
        if (!Object.HasInputAuthority) return;

        isChatOpen = !isChatOpen;

        // Bật/tắt UI chat
        inputFieldMessage.gameObject.SetActive(isChatOpen);
        chatBackground.gameObject.SetActive(isChatOpen);
        textMessage1.gameObject.SetActive(isChatOpen);

        // Tắt/bật script di chuyển
        if (movementScript != null)
        {
            movementScript.enabled = !isChatOpen;
        }

        Debug.Log("click chat: " + isChatOpen);
    }

    public void SendMessageChat()
    {
        if (!Object.HasInputAuthority) return;

        var message = inputFieldMessage.text;
        if (string.IsNullOrWhiteSpace(message)) return;

        var id = Runner.LocalPlayer.PlayerId;
        var text = $"s{id}: {message}";

        RpcChat(text);
        inputFieldMessage.text = "";
        Debug.Log($"Player {id} sent message: {text}");
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcChat(string message)
    {
        if (localPlayerChat != null)
        {
            localPlayerChat.DisplayMessage(message);
        }
    }

    public void DisplayMessage(string message)
    {
        textMessage2.gameObject.SetActive(true);
        textMessage1.text += message + "\n";
        textMessage2.text = message;
        StartCoroutine(HideTextMessageAfterDelay(5f));
    }

    private IEnumerator HideTextMessageAfterDelay(float time)
    {
        yield return new WaitForSeconds(time);
        textMessage2.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!Object.HasInputAuthority) return;

        if (Input.GetKeyDown(KeyCode.Return) && isChatOpen)
        {
            SendMessageChat();
        }

        if (isChatOpen && inputFieldMessage != null)
        {
            inputFieldMessage.ActivateInputField();
        }
    }
}
