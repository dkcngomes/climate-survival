"use client";

/**
 * Animated skeleton placeholders that mimic the actual card layout.
 * Makes loading feel fast and gives users a preview of the content structure.
 */

function SkeletonBlock({ className }: { className?: string }) {
  return (
    <div
      className={`skeleton rounded-lg ${className || ""}`}
      style={{ minHeight: "1rem" }}
    />
  );
}

export function RecommendationSkeleton() {
  return (
    <div className="rounded-2xl border-2 border-gray-200 p-5 bg-white">
      {/* Image placeholder */}
      <div className="w-full h-32 skeleton rounded-xl mb-3" />
      {/* Title row */}
      <div className="flex items-center gap-3 mb-3">
        <div className="w-10 h-10 skeleton rounded-full shrink-0" />
        <div className="flex-1">
          <SkeletonBlock className="h-5 w-3/4 mb-1" />
          <SkeletonBlock className="h-3 w-1/2" />
        </div>
        <div className="w-16 h-6 skeleton rounded-full" />
      </div>
      {/* Content lines */}
      <div className="space-y-2">
        <SkeletonBlock className="h-3 w-full" />
        <SkeletonBlock className="h-3 w-5/6" />
        <SkeletonBlock className="h-3 w-4/6" />
      </div>
      {/* Tags */}
      <div className="flex gap-2 mt-3">
        <SkeletonBlock className="h-6 w-20 rounded-full" />
        <SkeletonBlock className="h-6 w-24 rounded-full" />
      </div>
    </div>
  );
}

export function CropSkeleton() {
  return (
    <div className="rounded-2xl border-2 border-gray-200 p-4 bg-white">
      <div className="flex items-start gap-3 mb-3">
        <div className="w-16 h-16 skeleton rounded-xl shrink-0" />
        <div className="flex-1">
          <SkeletonBlock className="h-5 w-2/3 mb-1" />
          <SkeletonBlock className="h-3 w-1/3" />
          <div className="flex gap-2 mt-2">
            <SkeletonBlock className="h-5 w-14 rounded-full" />
            <SkeletonBlock className="h-5 w-14 rounded-full" />
          </div>
        </div>
      </div>
      <SkeletonBlock className="h-20 w-full rounded-lg" />
    </div>
  );
}

export function OverviewSkeleton() {
  return (
    <div className="bg-white rounded-2xl shadow-lg p-6 mb-8">
      <div className="flex items-start justify-between mb-6">
        <div>
          <SkeletonBlock className="h-7 w-48 mb-2" />
          <SkeletonBlock className="h-4 w-36 mb-1" />
          <SkeletonBlock className="h-3 w-28" />
        </div>
        <div className="text-right">
          <SkeletonBlock className="h-3 w-24 mb-1 ml-auto" />
          <SkeletonBlock className="h-6 w-12 ml-auto" />
        </div>
      </div>
      {/* Metrics grid */}
      <div className="grid grid-cols-2 gap-4 mb-6">
        <div className="bg-gray-50 rounded-xl p-4">
          <SkeletonBlock className="h-3 w-24 mb-2" />
          <SkeletonBlock className="h-8 w-20 mb-1" />
          <SkeletonBlock className="h-3 w-16" />
        </div>
        <div className="bg-gray-50 rounded-xl p-4">
          <SkeletonBlock className="h-3 w-24 mb-2" />
          <SkeletonBlock className="h-8 w-20 mb-1" />
          <SkeletonBlock className="h-3 w-16" />
        </div>
      </div>
      {/* Signal badges */}
      <div className="flex gap-2">
        <SkeletonBlock className="h-8 w-28 rounded-full" />
        <SkeletonBlock className="h-8 w-32 rounded-full" />
      </div>
    </div>
  );
}

export function ChartsSkeleton() {
  return (
    <div className="bg-white rounded-2xl border border-gray-200 p-5 shadow-sm">
      <SkeletonBlock className="h-6 w-40 mb-6" />
      <SkeletonBlock className="h-48 w-full rounded-xl mb-4" />
      <div className="grid grid-cols-2 gap-4">
        <SkeletonBlock className="h-32 rounded-xl" />
        <SkeletonBlock className="h-32 rounded-xl" />
      </div>
    </div>
  );
}
