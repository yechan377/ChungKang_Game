using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMover : MonoBehaviour
{
    public Transform character;
    public PathManager pathManager;
    public GameManager gameManager;
    public float moveSpeed = 3f;
    public float minScaleFactor = 0.3f;

    private Vector3 initialScale;

    public void SetCharacter(Transform newCharacter)
    {
        character = newCharacter;
        initialScale = character.localScale;
    }

    public void ResetCharacter(Vector3 position)
    {
        StopAllCoroutines();
        character.position = position;
        character.localScale = initialScale;
    }

    public void OnPlayButtonPressed()
    {
        if (gameManager.GetState() != GameManager.GameState.Drawing) return;

        gameManager.StartPlaying();
        character.localScale = initialScale;
        StartCoroutine(MoveAlongPath());
    }

    IEnumerator MoveAlongPath()
    {
        List<CurveDrawer> segments = pathManager.GetSegmentsInOrder();
        float totalLength = CalculateTotalLength(segments);
        float traveledDistance = 0f;

        for (int s = 0; s < segments.Count; s++)
        {
            CurveDrawer segment = segments[s];

            if (s > 0 && !segment.IsConnectedToPrevious())
            {
                gameManager.FailStage();
                yield break;
            }

            List<Vector3> points = segment.GetSmoothPoints();
            float traveledSegmentDist = 0f;
            float segmentLength = segment.GetTotalSmoothLength();

            if (segment.IsPortalJump() && points.Count > 0)
            {
                character.position = points[0];
            }

            for (int i = 0; i < points.Count; i++)
            {
                if (character == null || gameManager.GetState() != GameManager.GameState.Playing)
                {
                    yield break;
                }

                Vector3 target = points[i];
                Vector3 previous = character.position;
                float segDist = Vector3.Distance(previous, target);

                while (Vector3.Distance(character.position, target) > 0.01f)
                {
                    if (character == null || gameManager.GetState() != GameManager.GameState.Playing)
                    {
                        yield break;
                    }

                    character.position = Vector3.MoveTowards(character.position, target, moveSpeed * Time.deltaTime);

                    traveledSegmentDist += Vector3.Distance(previous, character.position);
                    segment.SetTraveledFraction(segmentLength > 0f ? traveledSegmentDist / segmentLength : 1f);

                    previous = character.position;
                    yield return null;
                }

                traveledDistance += segDist;
                float fraction = totalLength > 0f ? traveledDistance / totalLength : 1f;
                character.localScale = Vector3.Lerp(initialScale, initialScale * minScaleFactor, fraction);
            }
        }

        gameManager.FailStage();
    }

    float CalculateTotalLength(List<CurveDrawer> segments)
    {
        float total = 0f;
        foreach (CurveDrawer segment in segments)
        {
            List<Vector3> points = segment.GetSmoothPoints();
            for (int i = 0; i < points.Count - 1; i++)
            {
                total += Vector3.Distance(points[i], points[i + 1]);
            }
        }
        return total;
    }
}