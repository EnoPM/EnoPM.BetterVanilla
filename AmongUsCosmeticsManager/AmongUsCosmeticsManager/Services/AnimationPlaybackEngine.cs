using System;
using System.Collections.Generic;
using AmongUsCosmeticsManager.Models.Animation;

namespace AmongUsCosmeticsManager.Services;

public record PlaybackStep(AnimationNode SourceNode, byte[]? FrameData, int DurationMs);

public static class AnimationPlaybackEngine
{
    public static List<PlaybackStep> BuildPlan(IEnumerable<AnimationNode> nodes, int defaultFps)
    {
        var defaultDuration = Math.Max(1, 1000 / Math.Max(1, defaultFps));
        var steps = new List<PlaybackStep>();
        CollectSteps(nodes, steps, defaultDuration);
        return steps;
    }

    private static void CollectSteps(IEnumerable<AnimationNode> nodes, List<PlaybackStep> steps, int defaultDuration)
    {
        foreach (var node in nodes)
        {
            switch (node)
            {
                case FrameNode frame:
                    steps.Add(new PlaybackStep(
                        node,
                        frame.Data.Length > 0 ? frame.Data : null,
                        frame.EffectiveDurationMs));
                    break;

                case DelayNode delay:
                    steps.Add(new PlaybackStep(node, null, delay.DurationMs));
                    break;

                case LoopNode loop:
                    var loopSteps = new List<PlaybackStep>();
                    CollectSteps(loop.Children, loopSteps, defaultDuration);
                    var count = Math.Clamp(loop.Count, 1, 100);
                    for (var i = 0; i < count; i++)
                    {
                        // First iteration references the loop node for the playhead
                        if (i == 0 && loopSteps.Count > 0)
                            steps.AddRange(loopSteps);
                        else
                            steps.AddRange(loopSteps);
                    }
                    break;
            }
        }
    }
}
