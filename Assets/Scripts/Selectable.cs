using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class that defines a selectable object. Gets all necessary information about user input from selection manager through Unity Events and method calls
public class Selectable : MonoBehaviour
{
    [SerializeField]
    private SelectionManager selectionManager;
    [SerializeField]
    private bool hovered = false;
    [SerializeField]
    private bool selected = false;
    private float selectTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        if (selectionManager == null)
            selectionManager = FindAnyObjectByType(typeof(SelectionManager)) as SelectionManager;

        selectionManager.onClick.AddListener(Select);
    }

    // Update is called once per frame
    void Update()
    {
        Renderer painter = GetComponent<Renderer>();
        if (selected)
        {
            painter.material.color = selectionManager.selectColor;
            selectTimer += Time.deltaTime;
            if (selectTimer > selectionManager.selectionDisplayTime)
                Unselect();
        }
        else if (hovered)
            painter.material.color = selectionManager.hoverColor;
        else
            painter.material.color = selectionManager.defaultColor;
    }

    public void Hover()
    {
        hovered = true;
    }

    public void Unhover()
    {
        hovered = false;
    }

    void Select()
    {
        if (hovered)
        {
            Unhover();
            selected = true;
        }
    }

    void Unselect() {
        if (selected)
        {
            selected = false;
            selectTimer = 0f;
        }
    }

}
