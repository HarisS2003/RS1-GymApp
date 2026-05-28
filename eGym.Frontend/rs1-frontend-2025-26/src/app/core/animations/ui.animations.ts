import { animate, animateChild, query, stagger, style, transition, trigger } from '@angular/animations';

/** Product shop dialog — scale + fade on open/close. */
export const dialogPopIn = trigger('dialogPopIn', [
  transition(':enter', [
    style({ opacity: 0, transform: 'scale(0.95)' }),
    animate('260ms cubic-bezier(0.22, 1, 0.36, 1)', style({ opacity: 1, transform: 'scale(1)' })),
  ]),
  transition(':leave', [
    animate('180ms ease-in', style({ opacity: 0, transform: 'scale(0.95)' })),
  ]),
]);

/** Shop grid — stagger cards when list key changes (load / filter). */
export const productGridAnimation = trigger('productGrid', [
  transition('* => *', [
    query(
      ':enter',
      [
        style({ opacity: 0, transform: 'translateY(14px)' }),
        stagger(55, [
          animate('320ms cubic-bezier(0.22, 1, 0.36, 1)', style({ opacity: 1, transform: 'none' })),
        ]),
      ],
      { optional: true },
    ),
    query('@*', animateChild(), { optional: true }),
  ]),
]);

/** Single product card micro-interaction (optional per-card). */
export const productCardEnter = trigger('productCardEnter', [
  transition(':enter', [
    style({ opacity: 0, transform: 'translateY(10px)' }),
    animate('280ms cubic-bezier(0.22, 1, 0.36, 1)', style({ opacity: 1, transform: 'none' })),
  ]),
]);
