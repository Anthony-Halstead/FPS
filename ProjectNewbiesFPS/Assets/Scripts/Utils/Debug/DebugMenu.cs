using System.Reflection;
using TMPro;
using UnityEngine;

namespace DebugMenu
{
    [RequireComponent(typeof(DebugCommands))]
    public class DebugMenu : MonoBehaviour
    {

        [SerializeField] private bool debugMenuEnabled;
        [SerializeField] private GameObject debugPanel;
        [SerializeField] internal TMP_Text debugText;
        [SerializeField] internal TMP_InputField debugInput;

        [SerializeField] private DebugCommands _debugCommands;
        


        private void Awake()
        {
            debugMenuEnabled = false;
            debugPanel.SetActive(false);
            _debugCommands = GetComponent<DebugCommands>();
            _debugCommands.enabled = false;
        }

        private void Update()
        {
            if (Input.GetButtonDown("Debug"))
            {
                ToggleMenu();
            }
        }

        private void ToggleMenu()
        {
            debugMenuEnabled = !debugMenuEnabled;

            _debugCommands.enabled = debugMenuEnabled;
            debugPanel.SetActive(debugMenuEnabled);

            if (debugMenuEnabled)
            {
                debugInput.Select();
            }
            else
            {
                debugInput.DeactivateInputField();
            }
        }

        public void OnDebugInput()
        {
            
            string lastInput = debugInput.text;
            string[] words = lastInput.Split(" ");
            debugText.text += lastInput + "\n";
            RunCommand(words);
            debugInput.Select();
        }


        public void RunCommand(string[] words)
        {
            string command = words[0].ToLower();
            if (_debugCommands.commands.Exists(cmd => cmd.ToLower() == command))
            {
                if (words.Length > 2)
                {
                    int param2;
                    if (int.TryParse(words[2], out param2))
                    {
                        InvokeMethod(words[0], new object[] {words[1], param2});
                    }
                    else
                    {
                        debugText.text += $"<color=red>Invalid integer format for parameter 2.</color>\n";
                    }
                }
                else if (words.Length > 1)
                {
                    InvokeMethod(command, new object[] {words[1]});
                }
                else if (words.Length == 1)
                {
                    InvokeMethod(command, new object[] { });
                }
            }
            else
            {
                debugText.text += $"<color=red>       {words[0]} is an INVALID command.  </color>\n";
            }
        }
        
        
        void InvokeMethod(string methodName, object[] parameters)
        {
            MethodInfo methodInfo = _debugCommands.GetType().GetMethod(methodName);

            if (methodInfo != null)
            {

                if (parameters == null)
                {
                    parameters = new object[0];
                }

                if (methodInfo.GetParameters().Length == parameters.Length)
                {
                    methodInfo.Invoke(_debugCommands, parameters);
                }
                else
                {
                    Debug.LogError($"Parameter mismatch for method {methodName}");
                }
            }
            else
            {
                Debug.LogError("Method " + methodName + " notFound");
            }
        }
    }
    
}
