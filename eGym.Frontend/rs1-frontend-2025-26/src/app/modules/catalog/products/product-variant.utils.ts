/** Canonical size key so "450", "450g", "450 g" group as one size. */
export function canonicalSizeKey(size: string): string {
  const raw = size.trim().toLowerCase().replace(/\s+/g, '');
  if (!raw) return '';

  const match = raw.match(/^(\d+(?:[.,]\d+)?)(kg|g|ml|l|xl|xxl|xs|s|m)?$/i);
  if (!match) return raw;

  const num = match[1].replace(',', '.');
  const unit = (match[2] ?? 'g').toLowerCase();
  return `${num}${unit}`;
}

export function canonicalFlavorKey(flavor: string): string {
  return flavor.trim().toLowerCase();
}

/** Stored/display size label (e.g. 450 → 450g). */
export function normalizeSizeForStorage(size: string): string {
  const trimmed = size.trim();
  if (!trimmed) return trimmed;

  const key = canonicalSizeKey(trimmed);
  const match = key.match(/^(\d+(?:\.\d+)?)(kg|g|ml|l|xl|xxl|xs|s|m)?$/);
  if (!match) return trimmed;

  const num = match[1];
  const unit = match[2] ?? 'g';
  return `${num}${unit}`;
}

export function displaySizeLabel(
  canonicalKey: string,
  variants: ReadonlyArray<{ size: string }>,
): string {
  const labels = variants
    .filter((v) => canonicalSizeKey(v.size) === canonicalKey)
    .map((v) => v.size.trim())
    .filter(Boolean);

  if (!labels.length) return canonicalKey;

  const withUnit = labels.find((l) => /(?:kg|g|ml|l)\b/i.test(l));
  if (withUnit) return withUnit;

  return labels.sort((a, b) => b.length - a.length)[0];
}

export function uniqueFlavorLabels(
  variants: ReadonlyArray<{ color: string }>,
): string[] {
  const byKey = new Map<string, string>();

  for (const variant of variants) {
    const label = variant.color.trim();
    if (!label) continue;

    const key = canonicalFlavorKey(label);
    if (!byKey.has(key)) byKey.set(key, label);
  }

  return [...byKey.values()].sort((a, b) =>
    a.localeCompare(b, undefined, { sensitivity: 'base' }),
  );
}

export function findVariantBySelection<T extends { size: string; color: string }>(
  variants: ReadonlyArray<T>,
  canonicalSize: string,
  flavorLabel: string,
): T | null {
  const flavorKey = canonicalFlavorKey(flavorLabel);
  return (
    variants.find(
      (v) =>
        canonicalSizeKey(v.size) === canonicalSize &&
        canonicalFlavorKey(v.color) === flavorKey,
    ) ?? null
  );
}

export function hasDuplicateCanonicalVariants(
  variants: ReadonlyArray<{ size: string; color: string }>,
): boolean {
  const seen = new Set<string>();
  for (const v of variants) {
    const key = `${canonicalSizeKey(v.size)}|${canonicalFlavorKey(v.color)}`;
    if (seen.has(key)) return true;
    seen.add(key);
  }
  return false;
}
