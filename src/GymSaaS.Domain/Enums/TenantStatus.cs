namespace GymSaaS.Domain.Enums;

public enum TenantStatus
{
    PendingSetup = 0,   // registered, hasn't finished onboarding
    Active = 1,
    Suspended = 2,      // e.g. non-payment to Gusherlabs
    Cancelled = 3
}