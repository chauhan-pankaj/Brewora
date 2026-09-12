import { BottomNav } from "./BottomNav";
import { CartFab } from "./CartFab";
import { StatusBar } from "./StatusBar";

export function Screen({
  children,
  nav,
  lightStatus,
  className = "",
}: {
  children: React.ReactNode;
  nav?: boolean;
  lightStatus?: boolean;
  className?: string;
}) {
  return (
    <div className={`screen ${className}`}>
      <StatusBar light={lightStatus} />
      <div className={`screen-scroll ${nav ? "" : "no-nav"}`}>{children}</div>
      {nav ? <CartFab /> : null}
      {nav ? <BottomNav /> : null}
    </div>
  );
}
