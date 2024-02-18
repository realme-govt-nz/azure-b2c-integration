# Real Me (MTS) Assertion Service - Azure AD B2C integration

This projects explains how to configure Real Me as a SAML identity provider into Azure AD B2C. It uses Azure AD B2C custom policies.

In this tutorial:
- RealMe is treated as an external identity provider (IdP).
- The only configured IdP is RealMe and we don't ask user to provide any futher information so from a user perspective there is no interaction with Azure AD B2C. To configure more complex user journey, please refer to the [Useful links section](#Useful-links)

## Creating an Azure AD B2C Tenant

Follow [this tutorial](https://docs.microsoft.com/en-us/azure/active-directory-b2c/tutorial-create-tenant) to:

- Create a new Azure AD B2C tenant
- Link your Azure AD B2C tenant to a subscription

## Creating signing and encryption keys.

Follow [this tutorial](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-get-started-custom#add-signing-and-encryption-keys) to:

1. Sign in to the [Azure portal](https://portal.azure.com/) as the global administrator of your Azure AD B2C tenant.
2. Make sure you're using the directory that contains your Azure AD B2C tenant. Click the **Directory and subscription filter** in the top menu and choose the directory that contains your tenant.
3. Choose **All services** in the top-left corner of the Azure portal, search for and select **Azure AD B2C**.
4. On the Overview page, select **Identity Experience Framework**.

### Create a signing key used to sign the id_token return by Azure AD B2C

1. Select **Policy Keys** and then select **Add**.
2. For **Options**, choose `Generate`.
3. In **Name**, enter `TokenSigningKeyContainer`. The prefix `B2C_1A_` will be added automatically.
4. For **Key type**, select `RSA`.
5. For **Key usage**, select `Signature`.
6. Click **Create**.

### Create an encryption key used to encrypt the refresh_token return by Azure AD B2C

1. Select **Policy Keys** and then select **Add**.
2. For **Options**, choose `Generate`.
3. In **Name**, enter `TokenEncryptionKeyContainer`. The prefix `B2C_1A_` will be added automatically.
4. For **Key type**, select `RSA`.
5. For **Key usage**, select `Encryption`.
6. Click **Create**.

### Upload the certificate used between Azure AD B2C and RealMe to exchange data.

1. Download the `MTS-Post-Onboarding-Bundle-2023.zip` from the [RealMe Developer Website](https://developers.realme.govt.nz/try-it-out-now/) and unzip it.
2. Rename the file `mts_saml_sp.p12` to `mts_saml_sp.pfx`.
3. Select **Policy Keys** and then select **Add**.
4. For **Options**, choose `Upload`.
5. In **Name**, enter `SamlMessageSigning`. The prefix B2C_1A_ will be added automatically.
6. In **File upload**, select the `mts_saml_sp.pfx` file.
7. In **Password**, enter the password of the certificate (you can find this information in the `readme.txt` file in the `MTS-Post-Onboarding-Bundle-2023.zip` zipped file)
8. Click **Create**.

## Customizing the Custom policies files.
 
The policies files used in this tutorial have been modified from the [SocialAndLocalAccounts](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccounts) starter pack.

To know more about policies files, you can read the associated documentation: [Policy files](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-overview-custom#policy-files)

1. Download these files:
- [TrustFrameworkBase.xml](./RealMe-Assertion-MTS/TrustFrameworkBase.xml)
- [TrustFrameworkExtensions.xml](./RealMe-Assertion-MTS/TrustFrameworkExtensions.xml)
- [SignUpSignInRealMeAssertion.xml](./RealMe-Assertion-MTS/SignUpSignInRealMeAssertion.xml)

2. In these files, replace these parameters and save the files.:
- `yourtenant` with the name of your B2C tenant (without the `.onmicrosoft.com`)
- `yourEntityID` with a valid RealMe Issuer (see [RealMe request parameters](https://developers.realme.govt.nz/how-realme-works/realme-request-parameters)) in this format `https://www.agencyname.govt.nz/context/application-name`

3. Update the RealMe Assertion SAML Metadata:
- From the `MTS-Post-Onboarding-Bundle-2023.zip` (See downloaded RealMe Bundle file), open the `MTSIdPAssertionSAMLMetadata.xml` file.
- Copy the content of the file (do not copy the `<?xml version="1.0" encoding="UTF-8" standalone="yes"?>` line).
- Open the `TrustFrameworkExtensions.xml` as paste here inside the CDATA section:

    ```xml
    <Item Key="PartnerEntity"><![CDATA[
    Add RealMe Assertion Metadata Here
    ]]>
    ```
- To change the Assertion format to XML, replace the following lines:

    ```
    <OutputClaim ClaimTypeReferenceId="safeB64Identity" PartnerClaimType="urn:nzl:govt:ict:stds:authn:safeb64:attribute:igovt:IVS:Assertion:JSON:Identity" />
    <OutputClaim ClaimTypeReferenceId="safeB64Address" PartnerClaimType="urn:nzl:govt:ict:stds:authn:safeb64:attribute:NZPost:AVS:Assertion:JSON:Address" />
    ``` 
    with these lines:
    ```
    <OutputClaim ClaimTypeReferenceId="safeB64Identity" PartnerClaimType="urn:nzl:govt:ict:stds:authn:safeb64:attribute:igovt:IVS:Assertion:Identity" />
    <OutputClaim ClaimTypeReferenceId="safeB64Address" PartnerClaimType="urn:nzl:govt:ict:stds:authn:safeb64:attribute:NZPost:AVS:Assertion:Address" />
    ```
- Save your changes.

4. Upload the policies:
- On the Custom Policies page of Identity Experience Framework, select **Upload Policy**.
- In this order, upload `TrustFrameworkBase.xml`, `TrustFrameworkExtensions.xml`, `SignUpSignInRealMeAssertion.xml`.

## Upload the B2C metadata file to RealMe

1. Download the B2C metadata file (replace `yourtenant` with the name of your B2C tenant):
  `https://yourtenant.b2clogin.com/yourtenant.onmicrosoft.com/B2C_1A_SignUpSignInRealMeAssertion/samlp/metadata?idptp=RealMeAssertion-SAML2`

2. Open the file and remove the `<Signature>...</Signature>` tag.

3. Browse this url: https://mtscloud.realme.govt.nz/Assertion/Metadata/Validate
- Select the metadata file you want to upload then click **Upload File**.
- On the next page, click **Import** then **Continue**.
- Update your configuration: https://mtscloud.realme.govt.nz/Assertion/Metadata/SelectConfig
- Select `yourEntityID` in the **entity ID** field, and click **View**.
- Select `Low Strength` in the **Default Authentication Strength** dropdown. If you'd like to change the setting to `Moderate Strength`, you will have to update the `TrustFrameworkExtensions.xml` file. Search for **IncludeAuthnContextClassReferences** and change the value to `urn:nzl:govt:ict:stds:authn:deployment:GLS:SAML:2.0:ac:classes:ModStrength`.
- Change **Assertion Flow** to `AssertAndLogin`.
- Change **Reponse Format** to `JSON` (Recommended as the default, otherwise select `XML` if updated Assertions in previous steps).
- Make sure `Return LAT (AssertAndLogin or AssertAndAlwaysLogin flows)` is checked.
- Click **Update**.

## Testing the policy

To test the policy, create an application registration in the B2C. the token will be send to https://jwt.ms/.

1. In the B2C Tenant, Click on **Identity Experience Framework**.
2. Click on **Applications**.
3. On the application page, click on **Add**
4. On the application creation page
    - Enter `jwt.ms` in the **Name** field.
    - Select `Accounts in any identity provider or organizational directory (for authenticating users with user flows)` for **Supported account types**
    - Select `Web` for **Select a platform** and set value to `https://jwt.ms/`.
    - Click on **Register**
    - Select `Authentication` section under **Manage**, and check Access Tokens + ID tokens check boxes.
    - Click on **Save**

5. On the **Identity Experience Framework**, select the `B2C_1A_SignUpSignInRealMeAssertion` policy:
6. The previously created application should be preselected otherwise select `jwt.ms` in the **Select application** dropdown.
7. Click on the **Run now** button, you will be redirected to RealMe
8. On the RealMe website, fill the IVS attributes required
9. Click on `Generate FLT` to generate a new random FLT to use (optional)
10. Click on `Initiate SAML Response`, it will redirect you to the https://jwt.ms/ website.

You can inspect the token returned by B2C:
- The **sub** claim contains the B2C `objectid`.
- The **idp** claim contains the B2C `realme.govt.nz`.
- The **safeB64Identity** claim contains the RealMe `Verified Identity`.
- The **safeB64Address** claim contains the RealMe `Verified Address`.
- The **lat** claim contains the RealMe `LAT opaque token`. 
- The **issuerUserId** and **fit** claims will be returned correctly once integrated in the RealMe ITE environment.

Sample token:
{
  "alg": "RS256",
  "kid": "00000000000000000000000000",
  "typ": "JWT"
}.{
  "ver": "1.0",
  "iss": "https://tenantname.b2clogin.com/00000000-0000-0000-0000-000000000000/v2.0/",
  "sub": "00000000-0000-0000-0000-000000000000",
  "aud": "00000000-0000-0000-0000-000000000000",
  "exp": 1701141094,
  "acr": "b2c_1a_signupsigninrealmeassertion",
  "nonce": "defaultNonce",
  "iat": 1701137494,
  "auth_time": 1701137494,
  "issuerUserId": "AZUE0000000000000000000000000000000",
  "fit": "AZUE0000000000000000000000000000000",
  "safeB64Identity": "ABCDEFGHIJKL...",
  "safeB64Address": "ABCDEFGHIJKL...",
  "idp": "realme.govt.nz",
  "nbf": 1701137494
}.[Signature]

## Decoding the verified identity and address

To decode the identity and address as part of the B2C journey, you can refer to this link:
- [Walkthrough: Integrate REST API claims exchanges in your Azure AD B2C user journey as validation on user input](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-rest-api-validation-custom)

You can use this code snippet to decode the safe base64 identity and address (C#):

```
using System;
using System.Text;
...
private static string ConvertFromSafeBase64String(string safeb64)
{
  // See https://tools.ietf.org/html/rfc3548#section-4
  var base64 = safeb64.Replace("-", "+").Replace("_", "/");
  return Encoding.UTF8.GetString(Convert.FromBase64String(base64));
}
```

## Useful links

Azure Active Directory B2C:
- [Azure Active Directory B2C Overview](https://azure.microsoft.com/en-us/services/active-directory-b2c/)
- [Custom policies in Azure Active Directory B2C](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-overview-custom)
- [Define a SAML technical profile in an Azure Active Directory B2C custom policy](https://docs.microsoft.com/en-us/azure/active-directory-b2c/saml-technical-profile)
- [Azure Active Directory B2C: Collecting Logs](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-troubleshoot-custom)

Real Me:
- [RealMe for developers](https://developers.realme.govt.nz/)
- [RealMe assertion service MTS](https://mtscloud.realme.govt.nz/Assertion/home)
