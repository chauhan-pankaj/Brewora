export function StatusBar({ light }: { light?: boolean }) {
  return (
    <div className={light ? "status light" : "status"}>
      <span>9:41</span>
      <span>● ● ●</span>
    </div>
  );
}
