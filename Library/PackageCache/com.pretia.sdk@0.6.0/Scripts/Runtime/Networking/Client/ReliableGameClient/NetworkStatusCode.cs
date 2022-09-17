namespace PretiaArCloud.Networking
{
    public enum NetworkStatusCode : ushort
    {
        
        Success,
        InvalidJson,
        InvalidToken,
        TokenHolderAlreadyExists,
        DuplicateConnectionRequest,
        GamePlayerExistsInReconnect,
        GamePlayerNotFoundInReconnect,
        PacketWithoutConnection,
        GamePlayerNull,
        UnauthorizedSender,
        ResourceAllocationError,

        UserNameAlreadyTaken,
        AppNameAlreadyTaken,
        GroupNameAlreadyTaken,
        AppNotFound,
        UserNotFound,
        MapNotFound,
        TimestampOverdue,
        MessageNotFound,
        EmptyParameter,
        ConfigError,
        NoResult,

        InternalServerError,
        AppKeyMapKeyAlreadyExists,
        OrgMapKeyAlreadyExists,
        ExpiredToken,
        InvalidAppKey,
        Undefined,
        OrganizationNotFound,
        DbError,
        MemberNotFound,
        DeveloperSettingNotFound,
        MFANotActive,
        InvalidOtp,
        MFANotCompleted,
        TokenTooShort,

        InvalidMap,
        NotEnoughMaps,
        MapMergingFailed,

        //For access control
        InvalidMemberAclData,
        NotEnoughPrivilege,
        InvitationPending,

        //Functional Errors
        PointCloudFail = 5000,

        // Billing Errors
        BillingErrorBase = 10000,
        AlreadySubscribed,
        NoSuchPricingPlan,
        InvoiceIdNotFound,
        NoReceipt,

        DevPortalErrorBase = 20000,
        AlreadyAMember,
        OrgNameAlreadyExists,
        WrongOldPassword,
        InvalidCaptcha,

        MapErrorBase = 30000,
        MapInUse,
        PostProcessNotComplete,
        MapCouldNotBeSaved,


        End,
    }
}