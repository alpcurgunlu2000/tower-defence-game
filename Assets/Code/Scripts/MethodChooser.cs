using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MethodChooser : MonoBehaviour
{
    public void ChooseCommand(string command)
    {
        if (string.IsNullOrWhiteSpace(command))
        {
            Debug.Log("No command received.");
            return;
        }

        Debug.Log($"ChooseCommand called with: {command}");

        if (command.StartsWith("--COMMAND--"))
        {
            string data = command.Substring(11);
            Debug.Log($"Parsed command: {data}");

            string[] parts = data.Split('+');
            string commandName = parts[0];
            Debug.Log($"Command: {commandName}");

            // Parametreleri tut
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            for (int i = 1; i < parts.Length; i++)
            {
                string[] kv = parts[i].Split(':');
                if (kv.Length == 2)
                {
                    parameters[kv[0]] = kv[1];
                    Debug.Log($"Param: {kv[0]} = {kv[1]}");
                }
            }

            if (commandName == "game_commands")
            {
                if (parameters.ContainsKey("option") && parameters["option"].Trim() == "play")
                {
                    if (SceneManager.GetActiveScene().buildIndex == 0)
                    {
                        GameObject menuObj = GameObject.Find("MenuManager");
                        if (menuObj != null)
                        {
                            var menu = menuObj.GetComponent<MainMenu>();
                            if (menu != null)
                            {
                                menu.PlayGame();
                            }
                        }
                    }
                }
                else if (parameters.ContainsKey("option") && parameters["option"].Trim() == "exit")
                {
                    if (SceneManager.GetActiveScene().buildIndex == 0)
                    {
                        GameObject menuObj = GameObject.Find("MenuManager");
                        if (menuObj != null)
                        {
                            var menu = menuObj.GetComponent<MainMenu>();
                            if (menu != null)
                            {
                                menu.QuitGame();
                            }
                        }
                    }
                }
            }
            else if (commandName == "tower_commands")
            {
                if (parameters.ContainsKey("option") && parameters["option"] == "put"
                    && parameters.ContainsKey("place") && SceneManager.GetActiveScene().buildIndex == 1)
                {
                    string placeStr = parameters["place"];
                    if (int.TryParse(placeStr, out int placeNumber))
                    {
                        if (placeNumber >= 0 && placeNumber <= 191)
                        {
                            Debug.Log($"Putting tower at place {placeNumber}");
                            GameObject _plot = placeNumber == 0
                                ? GameObject.Find("Plot")
                                : GameObject.Find($"Plot ({placeStr.Trim()})");
                            
                            if (_plot != null)
                            {
                                _plot.GetComponent<Plot>().PutTower();
                            }
                            else
                            {
                                Debug.LogWarning($"Plot ({placeStr.Trim()}) not found!");
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"Invalid place number {placeNumber}, must be 0-191.");
                        }
                    }
                }
            }
else if (commandName == "turret_select_commands")
{
    if (parameters.ContainsKey("option") && parameters.ContainsKey("turret_color")
        && SceneManager.GetActiveScene().buildIndex == 1)
    {
        string color = parameters["turret_color"].Trim();
        string buttonName = $"Turret{UppercaseFirst(color)}";
        GameObject targetButtonObj = GameObject.Find(buttonName);

        if (targetButtonObj != null)
        {
            var buttonComponent = targetButtonObj.GetComponent<UnityEngine.UI.Button>();
            if (buttonComponent != null)
            {
                buttonComponent.onClick.Invoke();
                Debug.Log($"Simulated click on '{buttonName}' via voice command");
            }
            else
            {
                Debug.LogWarning($"Found '{buttonName}', but it has no Unity Button component.");
            }
        }
        else
        {
            Debug.LogWarning($"No turret button named '{buttonName}' found in hierarchy.");
        }
    }
}
        }
        else
        {
            Debug.Log("Invalid command format.");
        }
    }

    private string UppercaseFirst(string s)
    {
        if (string.IsNullOrEmpty(s)) return s;
        return char.ToUpper(s[0]) + s.Substring(1);
    }
}