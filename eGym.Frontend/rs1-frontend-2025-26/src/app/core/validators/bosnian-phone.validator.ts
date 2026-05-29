import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

const LOCAL_MOBILE_PREFIXES = new Set([
  '060', '061', '062', '063', '064', '065', '066', '067', '068', '069',
]);

const LOCAL_LANDLINE_PREFIXES = new Set(['033', '034', '035', '036', '037', '038', '039']);

function compact(input: string): string {
  return input.trim().replace(/[\s\-()]/g, '');
}

function isValidInternationalRest(rest: string): boolean {
  if (rest.length === 8) {
    return rest[0] === '6' || rest.startsWith('33');
  }
  if (rest.length === 9 && rest.startsWith('33')) {
    return true;
  }
  return false;
}

function tryNormalizeInternational(compact: string): string | null {
  const digits = compact.startsWith('+') ? compact.slice(1) : compact;
  if (!digits.startsWith('387')) {
    return null;
  }

  const rest = digits.slice(3);
  if ((rest.length !== 8 && rest.length !== 9) || !/^\d+$/.test(rest)) {
    return null;
  }
  if (!isValidInternationalRest(rest)) {
    return null;
  }

  return `+387${rest}`;
}

function tryNormalizeLocal(compact: string): string | null {
  if (!compact.startsWith('0') || compact.length !== 9 || !/^\d+$/.test(compact)) {
    return null;
  }

  const prefix = compact.slice(0, 3);
  if (!LOCAL_MOBILE_PREFIXES.has(prefix) && !LOCAL_LANDLINE_PREFIXES.has(prefix)) {
    return null;
  }

  return `+387${compact.slice(1)}`;
}

export function isValidBosnianPhone(input: string | null | undefined): boolean {
  return normalizeBosnianPhone(input) != null;
}

export function normalizeBosnianPhone(input: string | null | undefined): string | null {
  if (input == null || !String(input).trim()) {
    return null;
  }

  const value = compact(String(input));
  if (!value) {
    return null;
  }

  return tryNormalizeInternational(value) ?? tryNormalizeLocal(value);
}

export function bosnianPhoneValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const raw = control.value;
    if (raw == null || String(raw).trim() === '') {
      return null;
    }
    return isValidBosnianPhone(String(raw)) ? null : { bosnianPhone: true };
  };
}
