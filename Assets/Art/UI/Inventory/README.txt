Inventory UI — clean modular PNG assets

Copy these PNGs to Assets/Art/UI/Inventory/Clean/
All sprites have transparent outer corners. No key is baked into any image.

IMPORTANT: The generated reference illustration has baked-in slots and therefore is NOT suitable as a dynamic 100-slot panel. These are newly drawn, clean modular replacements inspired by its colors.

Unity import: Texture Type Sprite (2D and UI); Filter Mode Point (no filter); Compression None.

For frames, use Image Type Simple initially.
Use inventory_background.png for InventoryPanel.
Use slot_normal.png for InventorySlot prefab Image.
Do NOT use slot_selected.png until selection logic is implemented.
The key is rendered by InventoryUI from Key_ItemData.Icon (Key_8).
