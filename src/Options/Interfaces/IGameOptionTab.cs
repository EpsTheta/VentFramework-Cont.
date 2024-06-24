using System;
using System.Collections.Generic;
using UnityEngine;
using VentLib.Options.UI;
using VentLib.Utilities.Optionals;

namespace VentLib.Options.Interfaces;

public interface IGameOptionTab
{
    void Activate();

    void Deactivate();

    void AddEventListener(Action<IGameOptionTab> callback);

    void AddOption(GameOption option);

    void RemoveOption(GameOption option);

    protected void HandleClick();
    
    StringOption InitializeOption(StringOption sourceOption);

    void Setup(RolesSettingsMenu menu);

    void SetPosition(Vector2 position);

    void Show();
    
    void Hide();

    bool Ignore();

    /// <summary>
    /// Filters, or possibly renders options before being sent to the main renderer.
    /// Main renderer will then only render options returned by this function.
    /// </summary>
    /// <returns></returns>
    List<GameOption> PreRender();

    Optional<Vector3> GetPosition();

    List<GameOption> GetOptions();
}