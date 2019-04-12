### Azure Active Directory B2C - RealMe Integration

This wiki explains how to configure Real Me as a SAML identity provider into Azure AD B2C. It uses Azure AD B2C custom policies (and Azure Functions):

1. RealMe Login:
- [Configure only RealMe Login (MTS)](./Login-MTS-Configure-Only-RealMe.md)
- [Configure RealMe Login as an external SAML IdP (MTS)](./Login-MTS-Configure-RealMe-As-SAML-IdP.md)
- [ITE integration](./Login-ITE.md)

2. RealMe Assertion:
- [Configure only RealMe Assertion (MTS)](./Assertion-MTS-Configure-Only-RealMe.md)
- [Configure RealMe Assertion as an external SAML IdP (MTS)](./Assertion-MTS-Configure-RealMe-As-SAML-IdP.md)
- [ITE integration](./Assertion-ITE.md)

### D365 Portal

1. Complete the Azure AD B2C - RealMe integration first.

2. Before started, you need to edit the **Token Issuer** claim provider:
- Find the **TechnicalProfile** with `Id="JwtIssuer"`
- Add/Replace this item in the **Metadata** section: `<Item Key="IssuanceClaimPattern">AuthorityWithTfp</Item>`
- Re-upload your policy

for more information see [Manage SSO and token customization using custom policies in Azure Active Directory B2C](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-reference-manage-sso-and-token-configuration)

3. To configure D365 portal authentication with azure AD B2C, you can follow this link:
- [Azure AD B2C provider settings for portals](https://docs.microsoft.com/en-us/dynamics365/customer-engagement/portals/azure-ad-b2c)

### Useful links

Azure Active Directory B2C:
- [Azure Active Directory B2C Overview](https://azure.microsoft.com/en-us/services/active-directory-b2c/)
- [Custom policies in Azure Active Directory B2C](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-overview-custom)
- [Define a SAML technical profile in an Azure Active Directory B2C custom policy](https://docs.microsoft.com/en-us/azure/active-directory-b2c/saml-technical-profile)
- [Azure Active Directory B2C: Collecting Logs](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-troubleshoot-custom)

Real Me:
- [RealMe for developers](https://developers.realme.govt.nz/)
- [RealMe login service MTS](https://mts.realme.govt.nz/logon-mts/home)
- [RealMe assertion service MTS](https://mts.realme.govt.nz/realme-mts/home/information.xhtml)

