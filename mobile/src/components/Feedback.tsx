export function EmptyState({
  title,
  hint,
  action,
}: {
  title: string;
  hint?: string;
  action?: React.ReactNode;
}) {
  return (
    <div className="empty">
      <h3>{title}</h3>
      {hint ? <p style={{ marginTop: 8 }}>{hint}</p> : null}
      {action}
    </div>
  );
}

export function SkeletonList() {
  return (
    <div>
      {[1, 2, 3, 4].map((i) => (
        <div key={i} style={{ display: "flex", gap: 10, marginBottom: 14 }}>
          <div className="skel" style={{ width: 74, height: 74 }} />
          <div style={{ flex: 1 }}>
            <div className="skel" style={{ height: 12, width: "60%", marginBottom: 8 }} />
            <div className="skel" style={{ height: 10, width: "90%", marginBottom: 8 }} />
            <div className="skel" style={{ height: 10, width: "30%" }} />
          </div>
        </div>
      ))}
    </div>
  );
}

export function ConfirmDialog({
  open,
  title,
  message,
  confirmLabel = "Confirm",
  onConfirm,
  onCancel,
}: {
  open: boolean;
  title: string;
  message: string;
  confirmLabel?: string;
  onConfirm: () => void;
  onCancel: () => void;
}) {
  if (!open) return null;
  return (
    <div className="dialog">
      <div className="panel">
        <h3>{title}</h3>
        <p>{message}</p>
        <button className="primary" onClick={onConfirm}>
          {confirmLabel}
        </button>
        <button className="continue" onClick={onCancel}>
          Cancel
        </button>
      </div>
    </div>
  );
}
