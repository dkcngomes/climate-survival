"use client";

import { useEffect, useRef, ReactNode } from "react";

interface Props {
  children: ReactNode;
  className?: string;
  /** Delay in ms before the animation starts */
  delay?: number;
  /** Which side to slide from: "up" | "left" | "right" | "scale" | "fade" */
  animation?: "up" | "left" | "right" | "scale" | "fade";
}

/**
 * Wraps any element with a scroll-triggered reveal animation.
 * The child fades + slides in when it enters the viewport.
 */
export default function AnimatedSection({
  children,
  className = "",
  delay = 0,
  animation = "up",
}: Props) {
  const ref = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const el = ref.current;
    if (!el) return;

    // Set initial style based on animation type
    switch (animation) {
      case "up":
        el.style.opacity = "0";
        el.style.transform = "translateY(24px)";
        break;
      case "left":
        el.style.opacity = "0";
        el.style.transform = "translateX(-24px)";
        break;
      case "right":
        el.style.opacity = "0";
        el.style.transform = "translateX(24px)";
        break;
      case "scale":
        el.style.opacity = "0";
        el.style.transform = "scale(0.9)";
        break;
      case "fade":
        el.style.opacity = "0";
        el.style.transform = "none";
        break;
    }

    const observer = new IntersectionObserver(
      ([entry]) => {
        if (entry.isIntersecting) {
          setTimeout(() => {
            el.style.transition =
              "opacity 0.6s cubic-bezier(0.4, 0, 0.2, 1), transform 0.6s cubic-bezier(0.4, 0, 0.2, 1)";
            el.style.opacity = "1";
            el.style.transform =
              animation === "fade" ? "none" : "translateY(0) translateX(0) scale(1)";
          }, delay);
          observer.unobserve(el);
        }
      },
      { threshold: 0.1 }
    );

    observer.observe(el);
    return () => observer.disconnect();
  }, [delay, animation]);

  return (
    <div ref={ref} className={className}>
      {children}
    </div>
  );
}
