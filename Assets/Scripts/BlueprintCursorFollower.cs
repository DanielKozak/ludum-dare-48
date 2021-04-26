using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueprintCursorFollower
{
    static bool isBlueprintFollowingCursor = false;
    static GameObject blueprint;
    static SpriteRenderer sRenderer;
    public static int followMode = 0;
    static (int x, int y) blueprintAnchorPosition;
    static Vector2 mPivotOffset;

    static void StartCursorBlueprintFollow(Sprite blueprintSprite, int fMode, Vector2 pivotOffset)
    {
        followMode = fMode;
        mPivotOffset = pivotOffset;
        blueprint = new GameObject("blueprint", typeof(SpriteRenderer));
        sRenderer = blueprint.GetComponent<SpriteRenderer>();
        sRenderer.sprite = blueprintSprite;
        isBlueprintFollowingCursor = true;
        InputManager.SetKeyAction(KeyCode.Mouse1, new IActions.Action_ResetCursor());
        InputManager.SetKeyAction(KeyCode.Escape, new IActions.Action_ResetCursor());

    }
    public static void StartCursorBlueprintFollow(Sprite blueprintSprite, int fMode)
    {
        if (fMode == 4)
        {
            GameManager.Instance.SpawnTruck();
            followMode = 0;
            InputManager.ResetCursor();
            return;
        }

        StartCursorBlueprintFollow(blueprintSprite, fMode, new Vector2(0, 0));
    }

    public static void EndCursorBlueprintFollow()
    {
        if (!isBlueprintFollowingCursor) return;
        GameManager.Destroy(blueprint);
        isBlueprintFollowingCursor = false;
        followMode = 0;
        CanPlace = false;
    }
    public static void Update()
    {
        if (!isBlueprintFollowingCursor) return;
        (int x, int y) currentCellPos = MapUtils.GetCellFromMousePos();
        if (currentCellPos != blueprintAnchorPosition)
        {
            var newPos = MapUtils.GetWorldPosByCell(currentCellPos);
            blueprint.transform.position = new Vector3(newPos.x + mPivotOffset.x, newPos.y + mPivotOffset.y, -24);
            blueprintAnchorPosition = currentCellPos;
            switch (followMode)
            {
                case 1:
                    if (MapController.Instance.TerrainMap.GetValue(currentCellPos) == 2)
                    {
                        sRenderer.color = Color.green;
                        CanPlace = true;
                    }
                    else
                    {
                        sRenderer.color = Color.red;
                        CanPlace = false;

                    }
                    break;
                case 2:
                    if (MapController.Instance.TerrainMap.GetValue(blueprintAnchorPosition) == 2)
                    {

                        sRenderer.color = Color.red;
                        CanPlace = false;
                        return;
                    }
                    foreach (var item in MapUtils.GetNeighbours(currentCellPos.x, currentCellPos.y, MapController.Instance.MapW, MapController.Instance.MapH))
                    {
                        if (MapController.Instance.TerrainMap.GetValue(item) == 2)
                        {

                            sRenderer.color = Color.green;
                            CanPlace = true;
                            return;
                        }
                    }
                    sRenderer.color = Color.red;
                    CanPlace = false;

                    break;
                case 3:
                    CanPlace = true;
                    sRenderer.color = Color.green;

                    break;
                default:
                    break;
            }

        }
    }

    public static bool CanPlace = false;

}
