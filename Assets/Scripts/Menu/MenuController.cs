using UnityEngine;
using UnityEngine.UIElements;

public class MenuControl : MonoBehaviour
{
    public VisualElement ui;

    public Button Tienda;
    public Button Inventario;

    private VisualElement tiendaPanel;
    private VisualElement inventarioPanel;

    private bool isMenuOpen = false;

    private void Awake()
    {
        ui = GetComponent<UIDocument>().rootVisualElement;
        ui.style.display = DisplayStyle.None;

        Tienda = ui.Q<Button>("Tienda");
        Inventario = ui.Q<Button>("Inventario");

        tiendaPanel = ui.Q<VisualElement>("TiendaPanel");
        inventarioPanel = ui.Q<VisualElement>("InventarioPanel");
    }

    private void OnEnable()
    {
        Tienda.clicked += OnTiendaButtonClicked;
        Inventario.clicked += OnInventarioButtonClicked;
    }

    private void OnDisable()
    {
        Tienda.clicked -= OnTiendaButtonClicked;
        Inventario.clicked -= OnInventarioButtonClicked;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleMenu();
        }
    }

    void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;

        ui.style.display = isMenuOpen ? DisplayStyle.Flex : DisplayStyle.None;

        Time.timeScale = isMenuOpen ? 0f : 1f;

        UnityEngine.Cursor.visible = isMenuOpen;
        UnityEngine.Cursor.lockState = isMenuOpen
            ? CursorLockMode.None
            : CursorLockMode.Locked;
    }

    private void OnTiendaButtonClicked()
    {
        tiendaPanel.style.display = DisplayStyle.Flex;
        inventarioPanel.style.display = DisplayStyle.None;
    }

    private void OnInventarioButtonClicked()
    {
        inventarioPanel.style.display = DisplayStyle.Flex;
        tiendaPanel.style.display = DisplayStyle.None;
    }
}