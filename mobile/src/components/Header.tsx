import { useNavigate } from "react-router-dom";

export function Header({
  title,
  right,
  onBack,
}: {
  title: string;
  right?: React.ReactNode;
  onBack?: () => void;
}) {
  const nav = useNavigate();
  return (
    <div className="head">
      <button className="back" onClick={onBack ?? (() => nav(-1))} aria-label="Back">
        ‹
      </button>
      <h2>{title}</h2>
      <div style={{ minWidth: 32, textAlign: "right" }}>{right}</div>
    </div>
  );
}
