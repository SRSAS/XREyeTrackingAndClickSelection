using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class TestManager : MonoBehaviour
{
    [SerializeField]
    private SelectionManager selectionManager;
    [SerializeField]
    private Selectable target;
    private int targetId;
    [SerializeField]
    private TMP_Text text;
    [SerializeField]
    private TMP_Text timer;
    [SerializeField]
    private float countdownTime = 3f;
    private float timeRemaining = 0f;
    private bool timerRunning = false;

    private Selectable[] targets;

    private List<SelectionData> selections = new();
    private SelectionData currentSelection;
    private int selectionNumber = 1;

    private GameObject lastHovered = null;
    private float hoverStartTime = 0f;
    private float hoverTime = 0f;

    private float selectionStartTime = 0f;
    private float selectionTime = 0f;
    private bool endingSelection = false;

    // Start is called before the first frame update
    void Start()
    {
        targets = (Selectable[])Resources.FindObjectsOfTypeAll(typeof(Selectable));
        if (selectionManager == null)
            selectionManager = FindAnyObjectByType<SelectionManager>();
        selectionManager.onClick.AddListener(SelectionEnd);

        SelectTarget();
        currentSelection = new SelectionData(selectionNumber, targetId);
        selectionNumber++;

        ShuffleTargets();
        RunTimer();
    }

    // Update is called once per frame
    void Update()
    {
        if(timerRunning)
        {
            if (timeRemaining > 0f)
            {
                timeRemaining -= Time.deltaTime;
                timer.text = Mathf.FloorToInt(timeRemaining % 60).ToString();
            }
            else
            {
                timeRemaining = 0f;
                timerRunning = false;
                HideTimer();
                DisplayTargets();
                SelectionBegin();
            }
        }

        HoverCheck();
    }

    void HideTimer()
    {
        timer.GetComponent<TextMeshProUGUI>().enabled = false;
    }

    void DisplayTargets()
    {
        foreach (var t in targets)
        {
            t.GetComponent<Renderer>().enabled = true;
        }
    }

    void RunTimer()
    {
        timeRemaining = countdownTime;
        foreach (var t in targets)
        {
            t.GetComponent<Renderer>().enabled = false;
        }
        timer.GetComponent<TextMeshProUGUI>().enabled = true;
        timerRunning = true;
    }

    // This method is probably not an optimal way of shuffling the targets
    void ShuffleTargets()
    {
        for (int i = 0; i < targets.Length; i++)
        {
            var t = targets[i];
            var rando = targets[Random.Range(0, targets.Length)];
            (rando.transform.position, t.transform.position) = (t.transform.position, rando.transform.position);
        }
    }

    void SelectTarget()
    {
        // Select a random target from the selectionManager's list of targets
        targetId = Random.Range(0, targets.Length);
        target = targets[targetId];
        if (text != null)
            text.text = System.String.Format("Please select the {0}", target.name);
    }

    int FindTargetId(GameObject go)
    {
        if (go == null)
            return -1;
        Selectable s = go.GetComponent<Selectable>();
        for (int i = 0; i < targets.Length; i++) {
            if (targets[i] == s)
                return i;
        }
        return -1;
    }

    void SelectionBegin()
    {
        selectionStartTime = Time.realtimeSinceStartup;
        endingSelection = false;
    }

    void HoverCheck()
    {
        if (lastHovered != selectionManager.hovered)
        {
            if (lastHovered != null) {
                hoverTime = Time.realtimeSinceStartup - hoverStartTime;
                // Register hover
                currentSelection.hoveredIds.Add(FindTargetId(lastHovered));
                currentSelection.hoveredTimes.Add(hoverTime);
            }
            lastHovered = selectionManager.hovered;
            if (selectionManager.hovered != null) {
                hoverStartTime = Time.realtimeSinceStartup;
            }
        }
    }

    void SelectionEnd()
    {
        if (!endingSelection && !timerRunning)
        {
            endingSelection = true;
            if (lastHovered != null)
            {
                hoverTime = Time.realtimeSinceStartup - hoverStartTime;
                // Register hover
                currentSelection.hoveredIds.Add(FindTargetId(lastHovered));
                currentSelection.hoveredTimes.Add(hoverTime);
            }
            selectionTime = Time.realtimeSinceStartup - selectionStartTime;
            currentSelection.selectionTime = selectionTime;
            currentSelection.selectedId = FindTargetId(selectionManager.lastSelected);

            lastHovered = null;
            selectionStartTime = 0f;

            Debug.Log(currentSelection.ToString());

            selections.Add(currentSelection);

            if (currentSelection.selectedId == targetId)
                SelectTarget();
            if (text != null)
                text.text = System.String.Format("Please select the {0}", target.name);

            currentSelection = new SelectionData(selectionNumber, targetId);
            selectionNumber++;
            ShuffleTargets();
            RunTimer();
        }
    }
}

public struct SelectionData
{
    public int id;
    public int targetId;
    public List<int> hoveredIds;
    public List<float> hoveredTimes;
    public int selectedId;
    public float selectionTime;

    public SelectionData(int selectionId, int target)
    {
        id = selectionId;
        targetId = target;
        hoveredIds = new List<int>();
        hoveredTimes = new List<float>();
        selectedId = -1;
        selectionTime = 0;
    }

    public override readonly string ToString()
    {
        StringBuilder sb = new(System.String.Format("SELECTION REPORT\nSelection number: {0}\nTarget Goal: {1}\nHovered Over {2} Targets\n", id, targetId, hoveredIds.Count));
        int count = 0;
        float timeOverTarget = 0f;
        for (int i = 0; i < hoveredIds.Count; i++)
        {
            if (hoveredIds[i] == targetId)
            {
                count++;
                timeOverTarget += hoveredTimes[i];
            }
        }
        sb.AppendLine(System.String.Format("Hovered Over the Goal {0} Times", count));
        sb.AppendLine(System.String.Format("Hovered Over the Goal for {0} Seconds", timeOverTarget));
        sb.AppendLine(System.String.Format("Selection Lasted {0} Seconds", selectionTime));
        if (selectedId == targetId)
            sb.AppendLine("The CORRECT Target Was Selected");
        else
        {
            sb.AppendLine("The WRONG Target Was Selected");
            sb.AppendLine(System.String.Format("The Selected Target Was: {0}", selectedId));
        }
        sb.AppendLine("All Hovers:");
        for (int i = 0; i < hoveredIds.Count; i++)
        {
            sb.AppendLine(System.String.Format("Hovered Over Target Number {0} For {1} Seconds", hoveredIds[i], hoveredTimes[i]));
        }
        sb.AppendLine("----- END -----");
            return sb.ToString();
    }
}
