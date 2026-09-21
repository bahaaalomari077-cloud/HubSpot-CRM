namespace Crm.Domain.Enums;

/// <summary>
/// Pipeline transition rules for <see cref="ApplicationStage"/>.
///
/// Linear path:  New → CreatePapers → Invoice → InProgress → FinalInvoice
/// Terminal:     Won / Lost — reachable from any open stage, and final once set.
///
/// (Structure mirrors the requested expected → interested → applied → approved/rejected
///  model: no skipping, optional no-backward, terminal states are absorbing.)
/// </summary>
public static class StageTransition
{
    /// <summary>The ordered, non-terminal pipeline path.</summary>
    public static readonly ApplicationStage[] Path =
    {
        ApplicationStage.New,
        ApplicationStage.CreatePapers,
        ApplicationStage.Invoice,
        ApplicationStage.InProgress,
        ApplicationStage.FinalInvoice
    };

    public static bool IsTerminal(ApplicationStage stage) =>
        stage is ApplicationStage.Won or ApplicationStage.Lost;

    /// <summary>Position on the linear path; -1 for terminal stages.</summary>
    public static int IndexOf(ApplicationStage stage) => Array.IndexOf(Path, stage);

    /// <summary>
    /// Validates a move. Returns null when allowed, otherwise the reason it is rejected.
    /// </summary>
    /// <param name="allowBackward">
    /// When true, moving to an earlier stage is permitted (a correction). Default false.
    /// </param>
    public static string? Validate(
        ApplicationStage from,
        ApplicationStage to,
        bool allowBackward = false)
    {
        if (from == to)
            return $"Application is already in stage '{to}'.";

        // A closed deal is final — reopening needs an explicit, separate operation.
        if (IsTerminal(from))
            return $"Cannot move a closed application out of '{from}'.";

        // Won / Lost may be entered from any open stage.
        if (IsTerminal(to))
            return null;

        var fromIndex = IndexOf(from);
        var toIndex = IndexOf(to);

        if (fromIndex < 0 || toIndex < 0)
            return $"Stage '{to}' is not part of the pipeline.";

        // No skipping: forward moves must advance exactly one step.
        if (toIndex > fromIndex)
        {
            return toIndex == fromIndex + 1
                ? null
                : $"Cannot skip stages: '{from}' must move to '{Path[fromIndex + 1]}' first.";
        }

        // Backward move.
        return allowBackward
            ? null
            : $"Cannot move backward from '{from}' to '{to}'.";
    }
}
