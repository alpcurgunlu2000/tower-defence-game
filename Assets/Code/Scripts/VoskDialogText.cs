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

	// State
	bool goat_left;
	bool wolf_left;
	bool cabbage_left;
	bool man_left;

	void Awake()
	{
		VoskSpeechToText.OnTranscriptionResult += OnTranscriptionResult;
		ResetState();
	}

	void ResetState()
	{
		goat_left = true;
		wolf_left = true;
		cabbage_left = true;
		man_left = true;
	}

	void CheckState()
	{
		if (goat_left && wolf_left && !man_left)
		{
			AddFinalResponse("волк съел козу, начинай сначала");
			return;
		}
		if (goat_left && cabbage_left && !man_left)
		{
			AddFinalResponse("коза съела капусту, начинай сначала");
			return;
		}
		if (!goat_left && !wolf_left && man_left)
		{
			AddFinalResponse("волк съел козу, начинай сначала");
			return;
		}
		if (!goat_left && !cabbage_left && man_left)
		{
			AddFinalResponse("коза съела капусту, начинай сначала");
			return;
		}
		if (!goat_left && !wolf_left && !cabbage_left && !man_left)
		{
			AddFinalResponse("отлично получилось, давай ещё раз!");
			return;
		}

		AddResponse("так, и что дальше");
	}

	void Say(string response)
	{
		// System.Diagnostics.Process.Start("/usr/bin/say", response); 
	}

	void AddFinalResponse(string response)
	{
		Say(response);
		DialogText.text = response + "\n";
		ResetState();
	}

	void AddResponse(string response)
	{
		Say(response);
		DialogText.text = response + "\n\n";

		DialogText.text += "крестьянин " + (man_left ? "слева" : "справа") + "\n";
		DialogText.text += "волк " + (wolf_left ? "слева" : "справа") + "\n";
		DialogText.text += "коза " + (goat_left ? "слева" : "справа") + "\n";
		DialogText.text += "капуста " + (cabbage_left ? "слева" : "справа") + "\n";

		DialogText.text += "\n";
	}

	public GeminiCommandProcessor geminiProcessor;

public MethodChooser methodChooser;

private async void OnTranscriptionResult(string obj)
{
    Debug.Log("Recognized JSON: " + obj);
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
}

}
