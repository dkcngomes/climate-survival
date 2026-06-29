"use client";

/**
 * AI Thinking Indicator — animated neural pulse + orbiting dots
 * to show the AI is actively processing recommendations.
 */
export default function AiThinkingIndicator() {
  return (
    <div className="flex flex-col items-center justify-center py-16">
      {/* Neural ring animation */}
      <div className="relative w-24 h-24 mb-6">
        {/* Outer pulsing ring */}
        <div className="absolute inset-0 rounded-full border-4 border-emerald-300 animate-ping opacity-25" />
        {/* Orbiting dot 1 */}
        <div
          className="absolute top-1/2 left-1/2 w-3 h-3 bg-emerald-500 rounded-full"
          style={{
            marginTop: "-6px",
            marginLeft: "-6px",
            animation: "orbit 1.8s linear infinite",
          }}
        />
        {/* Orbiting dot 2 (reverse) */}
        <div
          className="absolute top-1/2 left-1/2 w-2.5 h-2.5 bg-emerald-400 rounded-full"
          style={{
            marginTop: "-5px",
            marginLeft: "-5px",
            animation: "orbit-reverse 2.2s linear infinite",
          }}
        />
        {/* Center icon */}
        <div className="absolute inset-0 flex items-center justify-center">
          <span className="text-3xl animate-pulse-soft">🧠</span>
        </div>
      </div>

      {/* AI label with typing dots */}
      <div className="flex items-center gap-2">
        <span className="inline-flex items-center gap-1.5 px-4 py-1.5 rounded-full bg-gradient-to-r from-emerald-500 to-emerald-600 text-white text-sm font-semibold shadow-lg animate-pulse-glow">
          <span className="text-base">✨</span>
          AI Processing
        </span>
      </div>

      {/* Processing steps */}
      <div className="flex items-center gap-3 mt-4 text-xs text-gray-500">
        <span className="flex items-center gap-1">
          <span className="w-1.5 h-1.5 bg-emerald-500 rounded-full animate-bounce" style={{ animationDelay: "0ms" }} />
          Analyzing climate
        </span>
        <span className="text-gray-300">•</span>
        <span className="flex items-center gap-1">
          <span className="w-1.5 h-1.5 bg-emerald-500 rounded-full animate-bounce" style={{ animationDelay: "200ms" }} />
          Computing risks
        </span>
        <span className="text-gray-300">•</span>
        <span className="flex items-center gap-1">
          <span className="w-1.5 h-1.5 bg-emerald-500 rounded-full animate-bounce" style={{ animationDelay: "400ms" }} />
          Generating insights
        </span>
      </div>

      {/* Gemini credit */}
      <p className="text-[10px] text-gray-400 mt-4 tracking-wider uppercase">
        Powered by Gemini AI ✦
      </p>
    </div>
  );
}
