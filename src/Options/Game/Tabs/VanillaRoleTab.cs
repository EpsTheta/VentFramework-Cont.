using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VentLib.Options.Game.Impl;
using VentLib.Options.Game.Interfaces;
using VentLib.Utilities.Optionals;
using Object = UnityEngine.Object;

namespace VentLib.Options.Game.Tabs;

public sealed class VanillaRoleTab : VanillaTab
{
    public static VanillaRoleTab Instance = null!;
    private static IVanillaMenuRenderer _renderer = new RoleMenuRenderer();
    
    private UnityOptional<SpriteRenderer> glyphL = UnityOptional<SpriteRenderer>.Null();
    private UnityOptional<SpriteRenderer> glyphR = UnityOptional<SpriteRenderer>.Null();
    private UnityOptional<RolesSettingsMenu> innerMenu = UnityOptional<RolesSettingsMenu>.Null();

    public VanillaRoleTab()
    {
        Instance = this;
    }
    
    public static void SetRenderer(IVanillaMenuRenderer renderer)
    {
        _renderer = renderer;
    }
    
    public override StringOption InitializeOption(StringOption sourceBehavior)
    {
        if (!innerMenu.Exists()) throw new ArgumentException("Cannot Initialize Behaviour because menu does not exist");
        return Object.Instantiate(sourceBehavior, innerMenu.Get().transform);
    }

    public override void Setup(MenuInitialized initialized)
    {
        TabButton = UnityOptional<GameObject>.NonNull(initialized.RoleTab);
        RelatedMenu = UnityOptional<GameObject>.NonNull(initialized.GameSettingMenu.RoleSettingsTab.gameObject);
        glyphL = UnityOptional<SpriteRenderer>.Of(initialized.GameSettingMenu.RoleSettingsTab.glyphL);
        glyphR = UnityOptional<SpriteRenderer>.Of(initialized.GameSettingMenu.RoleSettingsTab.glyphR);
        innerMenu = RelatedMenu.UnityMap(menu => menu.transform.GetComponentsInChildren<Transform>().First(c => c.name.Equals("SliderInner")).GetComponent<RolesSettingsMenu>());

        var button = initialized.RoleTab.GetComponentInChildren<PassiveButton>();
        button.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
        button.OnClick.AddListener((Action)HandleClick);
    }

    public override List<GameOption> PreRender()
    {
        var returnList = new List<GameOption>();
        if (!innerMenu.Exists()) return returnList;
    
        List<GameOption> options = GetOptions().SelectMany(opt => opt.GetDisplayedMembers()).ToList();
        
        if (options.Count == 0) return returnList;
        var menu = innerMenu.Get();
        
        _renderer.Render(menu, options, menu.advancedSettingChildren.ToArray().Skip(1), GameOptionController.RenderOptions);
        
        return new List<GameOption>();
    }

    protected override void SetGlyphEnabled(bool enabled)
    {
        if (!innerMenu.Exists()) 
        {
            glyphL.IfPresent(g => g.enabled = enabled);
            glyphR.IfPresent(g => g.enabled = enabled);
        } else {
            var menu = innerMenu.Get();

            glyphL.IfPresent(g => g.enabled = enabled && menu.selectedRoleTab > 0);
            glyphR.IfPresent(g => g.enabled = enabled && menu.selectedRoleTab < menu.roleTabs.Count - 1);
        }
    }
}