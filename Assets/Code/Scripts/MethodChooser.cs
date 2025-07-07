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
                    if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 0)
                    {
                        GameObject menuObj = GameObject.Find("MenuManager");
                        if (menuObj != null)
                        {
                            var menu = menuObj.GetComponent<MainMenu>();
                            if (menu != null)
                            {
                                menu.PlayGame();
                            }
                            else
                            {
                                Debug.LogError("MainMenu component not found!");
                            }
                        }
                        else
                        {
                            Debug.LogError("MainMenu object not found!");
                        }
                    }
                }
                else if (parameters.ContainsKey("option") && parameters["option"].Trim() == "exit")
                {
                    if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 0)
                    {
                        GameObject menuObj = GameObject.Find("MenuManager");
                        if (menuObj != null)
                        {
                            var menu = menuObj.GetComponent<MainMenu>();
                            if (menu != null)
                            {
                                menu.QuitGame();
                            }
                            else
                            {
                                Debug.LogError("MainMenu component not found!");
                            }
                        }
                        else
                        {
                            Debug.LogError("MainMenu object not found!");
                        }
                    }
                }
            }

            else if (commandName == "tower_commands")
            {
                if (parameters.ContainsKey("option") && parameters["option"] == "put"
                    && parameters.ContainsKey("place") && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 1)
                {
                    string placeStr = parameters["place"];
                    if (int.TryParse(placeStr, out int placeNumber))
                    {
                        if (placeNumber >= 0 && placeNumber <= 191)
                        {
                            Debug.Log($"Putting tower at place {placeNumber}");
                            GameObject _plot;
                            if (placeNumber == 0)
                            {
                                _plot = GameObject.Find("Plot");
                            }
                            else
                            {
                                _plot = GameObject.Find("Plot (" + placeStr.Trim() + ")");
                            }
                            _plot.GetComponent<Plot>().PutTower();
                            
                        }
                        else
                        {
                            Debug.LogWarning($"Invalid place number {placeNumber}, must be 1-191.");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Place value '{placeStr}' is not a valid number.");
                    }
                }
                else
                {
                    Debug.LogWarning("tower_commands requires option=put and a valid place.");
                }
            }

        }

        else
        {
            Debug.Log("Invalid command format.");
        }
    }
}
