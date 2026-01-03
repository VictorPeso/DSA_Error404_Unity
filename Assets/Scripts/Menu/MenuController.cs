using UnityEngine;
using UnityEngine.UIElements;

public class MenuController : MonoBehaviour
{
    public VisualElement ui;

    public Button Tienda;
    public Button Inventario;

    private VisualElement Panel;
    private VisualElement tiendaPanel;
    private VisualElement inventarioPanel;

    public VisualTreeAsset tiendaUXML;

    private bool isMenuOpen = false;

    private void Awake()
    {
        UIDocument doc = GetComponent<UIDocument>();
        if (doc == null)
        {
            Debug.LogError("MenuControl: No hay UIDocument en este GameObject");
            return;
        }

        ui = doc.rootVisualElement;
        ui.style.display = DisplayStyle.None;

        Panel = ui.Q<VisualElement>("Panel");

        Tienda = ui.Q<Button>("Tienda");
        Inventario = ui.Q<Button>("Inventario");

        tiendaPanel = ui.Q<VisualElement>("TiendaPanel");
        inventarioPanel = ui.Q<VisualElement>("InventarioPanel");
        
        if (Tienda == null || Inventario == null)
            Debug.LogError("MenuControl: Botones no encontrados en UXML");

        if (tiendaPanel == null || inventarioPanel == null)
            Debug.LogError("MenuControl: Paneles no encontrados en UXML");

        tiendaPanel.style.display = DisplayStyle.None;
        inventarioPanel.style.display = DisplayStyle.None;
    }

    private void OnEnable()
    {
        if (Tienda != null)
            Tienda.clicked += OnTiendaButtonClicked;

        if (Inventario != null)
            Inventario.clicked += OnInventarioButtonClicked;
    }

    private void OnDisable()
    {
        if (Tienda != null)
            Tienda.clicked -= OnTiendaButtonClicked;

        if (Inventario != null)
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
        if (ui == null) return;

        isMenuOpen = !isMenuOpen;
        ui.style.display = isMenuOpen ? DisplayStyle.Flex : DisplayStyle.None;

        Time.timeScale = isMenuOpen ? 0f : 1f;

        UnityEngine.Cursor.visible = isMenuOpen;
        UnityEngine.Cursor.lockState = isMenuOpen
            ? CursorLockMode.None
            : CursorLockMode.Locked;

        if (!isMenuOpen)
        {
            tiendaPanel.style.display = DisplayStyle.None;
            inventarioPanel.style.display = DisplayStyle.None;
        }
    }

    private void OnTiendaButtonClicked()
    {
        if (tiendaPanel == null || inventarioPanel == null) return;

        Panel.style.display = DisplayStyle.None;
        tiendaPanel.style.display = DisplayStyle.Flex;
        inventarioPanel.style.display = DisplayStyle.None;


    }

    private void OnInventarioButtonClicked()
    {
        if (tiendaPanel == null || inventarioPanel == null) return;

        Panel.style.display = DisplayStyle.None;
        inventarioPanel.style.display = DisplayStyle.Flex;
        tiendaPanel.style.display = DisplayStyle.None;
    }
}