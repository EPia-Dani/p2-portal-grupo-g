using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    [SerializeField] private Image crosshairImage;
    [SerializeField] private Sprite crosshairTextureNone;
    [SerializeField] private Sprite crosshairTextureBlue;
    [SerializeField] private Sprite crosshairTextureOrange;
    [SerializeField] private Sprite crosshairTextureBoth;
    
    void OnEnable()
    {
        PortalPlacer.OnCrosshairChange += HandleCrosshairChange;
    }

    void OnDisable()
    {
        PortalPlacer.OnCrosshairChange -= HandleCrosshairChange;
    }

    private void HandleCrosshairChange(PortalPlacer.PortalType type)
    {
        Debug.Log("Crosshair changed to: " + type);
        switch (type)
        {
            case PortalPlacer.PortalType.None:
                crosshairImage.sprite = crosshairTextureNone;
                break;
            case PortalPlacer.PortalType.Blue:
                crosshairImage.sprite = crosshairTextureBlue;
                break;
            case PortalPlacer.PortalType.Orange:
                crosshairImage.sprite = crosshairTextureOrange;
                break;
            case PortalPlacer.PortalType.Both:
                crosshairImage.sprite = crosshairTextureBoth;
                break;
        }
    }

    
}
