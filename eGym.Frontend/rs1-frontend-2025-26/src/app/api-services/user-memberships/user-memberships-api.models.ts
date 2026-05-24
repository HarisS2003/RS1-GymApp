/** PaymentMethod.Cash — only supported method for now */
export const PAYMENT_METHOD_CASH = 2;

export interface PurchaseMembershipPlanCommand {
  membershipPlanId: number;
  paymentMethod: number;
}

export interface PurchaseMembershipPlanResultDto {
  userMembershipId: number;
  paymentId: number;
  membershipPlanId: number;
  planName: string;
  amountPaid: number;
  startDate: string;
  endDate: string;
  paymentMethod: number;
  paymentStatus: number;
}

export interface GetMyActiveUserMembershipQueryDto {
  userMembershipId: number;
  membershipPlanId: number;
  planName: string;
  durationDays: number;
  price: number;
  discountPercentage: number;
  finalPrice: number;
  startDate: string;
  endDate: string;
}

export interface ListMyMembershipPurchaseHistoryQueryDto {
  userMembershipId: number;
  planName: string;
  amountPaid: number;
  purchasedAt: string;
  endDate: string;
  durationDays: number;
  isActive: boolean;
}
