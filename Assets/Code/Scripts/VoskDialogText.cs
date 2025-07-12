using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class VoskDialogText : MonoBehaviour
{
	public VoskSpeechToText VoskSpeechToText;
	public Text DialogText;

	Regex hi_regex = new Regex(@"привет");
	Regex who_regex = new Regex(@"кто ты");
	Regex pass_regex = new Regex("(хорошо|давай)");
	Regex help_regex = new Regex("помоги");

	Regex goat_regex = new Regex(@"(козу|начнём с козы)");
	Regex wolf_regex = new Regex(@"(волк|волка)");
	Regex cabbage_regex = new Regex(@"(капуста|капусту|начнём с капусты)");

	Regex goat_back_regex = new Regex(@"(козу назад|вернём козу|верни козу)");
	Regex wolf_back_regex = new Regex(@"(волка назад|вернём волка|верни волка)");
	Regex cabbage_back_regex = new Regex(@"(капусту назад|вернём капусту|верни капусту)");

	Regex forward_regex = new Regex("переедем");
	Regex back_regex = new Regex("(назад|вернёмся назад)");

	void Awake()
	{
		VoskSpeechToText.OnTranscriptionResult += OnTranscriptionResult;
	}

	public GeminiCommandProcessor geminiProcessor;

public MethodChooser methodChooser;

/* 
	select a turret (modality fission)
	- 1. speech: give me red tower here (turret selection command) -> 2.5 seconds
        - record the speech (raw speech) -> 1-2 seconds till user completes its speech (user interaction)
        - process the speech() -> get the command type on this process. async function (background thread / no user interaction / async)
    - 2. mouse-click: click blue tower -> it should ignore this click

	if the speech is false alarm may be noise
	- 1. speech: sound is falsy may be noise
	- 2. mouse-click: click blue tower -> it should trigger the command
*/
private async void OnTranscriptionResult(string obj)
{
    var result = new RecognitionResult(obj);

    if (result.Phrases.Length == 0 || string.IsNullOrWhiteSpace(result.Phrases[0].Text))
    {
        Debug.Log("No phrases recognized.");
        return;
    }

    string recognizedText = result.Phrases[0].Text;
    Debug.Log("Recognized sentence: " + recognizedText);

    if (geminiProcessor == null)
    {
        Debug.LogError("geminiProcessor is not set in Inspector!");
        return;
    }

    string commandOutput = await geminiProcessor.GetCommand(recognizedText);
    Debug.Log("Generated command from Gemini: " + commandOutput);

    if (methodChooser != null)
    {
        methodChooser.ChooseCommand(commandOutput);
    }
    else
    {
        Debug.LogError("MethodChooser not assigned in inspector!");
    }
    if (DialogText != null)
        DialogText.text = $"Sentence: {recognizedText}\nCommand: {commandOutput}";

    // Complete speech processing with the command result
    if (MouseActionDelayer.Instance != null)
    {
        MouseActionDelayer.Instance.CompleteSpeechProcessing(commandOutput);
    }
}

}
