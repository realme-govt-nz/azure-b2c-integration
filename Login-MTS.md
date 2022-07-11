# Real Me (MTS) Login Service - Azure AD B2C integration

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

### Upload the certificate used between Azure AD B2C and Relame to exchange data.

1. Download the `Updated-MTS-POST-Binding-Bundle-Oct-2021.zip` from the [RealMe Developer Website](https://developers.realme.govt.nz/try-it-out-now/) and unzip it.
2. Rename the file `mts_saml_sp.p12` to `mts_saml_sp.pfx`.
3. Select **Policy Keys** and then select **Add**.
4. For **Options**, choose `Upload`.
5. In **Name**, enter `SamlMessageSigning`. The prefix B2C_1A_ might be added automatically.
6. In **File upload**, select the `mts_mutual_ssl_sp.pfx` file.
7. In **Password**, enter the password of the certificate (you can find this information in the `readme.txt` file in the `Updated-MTS-POST-Binding-Bundle-Oct-2021.zip` zipped file or just try 'password')
8. Click **Create**.

## Customizing the Custom policies files.
 
The policies files used in this tutorial have been modified from the [SocialAndLocalAccounts](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccounts) starter pack.

To know more about policies files, you can read the associated documentation: [Policy files](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-overview-custom#policy-files)

1. Download these files:
- [TrustFrameworkBase.xml](./RealMe-Login-MTS/TrustFrameworkBase.xml)
- [TrustFrameworkExtensions.xml](./RealMe-Login-MTS/TrustFrameworkExtensions.xml)
- [SignUpSignInRealMeLogin.xml](./RealMe-Login-MTS/SignUpSignInRealMeLogin.xml)

2. In these files, replace these parameters and save the files.:
- `yourtenant` with the name of your B2C tenant (without the `.onmicrosoft.com`)
- `yourEntityID` with a valid RealMe Issuer (see [RealMe request parameters](https://developers.realme.govt.nz/how-realme-works/realme-request-parameters)) in this format `https://www.agencyname.govt.nz/context/application-name`

3. Update the RealMe Login SAML Metadat
- From the `MTS RealMe Replatforming Bundle - POST Binding.zip` (See previous step), open the `Realme_IDP_Metadata_LoginService.xml` file.
- Copy the content of the file (do not copy the `<?xml version="1.0" encoding="UTF-8" standalone="yes"?>` line).
- Open the `TrustFrameworkExtensions.xml` and past here:

    ```xml
    <Item Key="PartnerEntity"><![CDATA[
    Add RealMe Metadata Here
    ]]>
    ```
- Save your changes.

4. Upload the policies:
- On the Custom Policies page of Identity Experience Framework, select **Upload Policy**.
- In this order, upload `TrustFrameworkBase.xml`, `TrustFrameworkExtensions.xml`, `SignUpSignInRealMeLogin.xml`.

## Upload the B2C metadata file to RealMe

1. Download the B2C metadata file (replace `yourtenant` with the name of your B2C tenant):
  `https://yourtenant.b2clogin.com/yourtenant.onmicrosoft.com/B2C_1A_SignUpSignInRealMeLogin/samlp/metadata?idptp=RealMeLogin-SAML2`
- If you want to use the `login.microsoftonline.com` domain, download the metadata file from this url (replace `yourtenant` with the name of your B2C tenant):
  `https://login.microsoftonline.com/te/yourtenant.onmicrosoft.com/B2C_1A_SignUpSignInRealMeLogin/samlp/metadata?idptp=RealMeLogin-SAML2`

2. Open the file and remove the `<Signature>...</Signature>` tag.

3. Browse this url: [https://mtscloud.realme.govt.nz/Login/Metadata/Validate)](https://mtscloud.realme.govt.nz/Login/Metadata/Validate)
- Select the metadata file you want to upload then click **Upload File**.
- On the next page, click **Import** then **Continue**.
- Update your configuration: [https://mtscloud.realme.govt.nz/Login/Metadata/UpdateConfig](https://mtscloud.realme.govt.nz/Login/Metadata/UpdateConfig)
- Select `yourEntityID` in the **entity ID** field.
- Select `Low Strength` in the **Default Authentication Strength** dropdown. If you'd like to change the setting to `Moderate Strength`, you will have to update the `TrustFrameworkExtensions.xml` file. Search for **IncludeAuthnContextClassReferences** and change the value to `urn:nzl:govt:ict:stds:authn:deployment:GLS:SAML:2.0:ac:classes:ModStrength`.
- Select `ICMS XML` in the **Login Attrbiutes Token Return Type** dropdown.
- Click **Update**.

## Testing the policy

To test the policy, create an application registration in the B2C. the token will be send to https://jwt.ms/.

1. In the B2C Tenant, Click on **Indentiy Experience Framework**.
2. Click on **Applications**.
3. On the application page, clieck on **Add**
4. On the application creation page
    - Enter `jwt.ms` in the **Name** field.
    - Select `Yes` for **Include web app / web API**
    - Select `Yes` for **allow implicit flow**
    - Enter `https://jwt.ms/` 
    - Click on **Create**

5. On the  **Indentiy Experience Framework**, select the `B2C_1A_SignUpSignInRealMeLogin` policy:
6. The previously created application should be preselected otherwithe select `jwt.ms` in the **Select application** dropdown.
7. Select the domain you want to use. This should be the based on the metadata file you've uploaded to realme (see [step 1](#-4.-Upload-the-B2C-metadata-file-to-RealMe))
8. Click on the **Run now** button, you will be redirected to RealMe
9. On the RealMe website, click on `Initiate SAML`, it will redirect you to the https://jwt.ms/ website.

You can inspect the token returned by B2C:
- The **sub** claim contains the B2C `objectid`.
- The **idp** claim contains the B2C `realme.govt.nz`.
- The **issuerUserId** claim contains the RealMe `FLT`.

## Useful links

Azure Active Directory B2C:
- [Azure Active Directory B2C Overview](https://azure.microsoft.com/en-us/services/active-directory-b2c/)
- [Custom policies in Azure Active Directory B2C](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-overview-custom)
- [Define a SAML technical profile in an Azure Active Directory B2C custom policy](https://docs.microsoft.com/en-us/azure/active-directory-b2c/saml-technical-profile)
- [Azure Active Directory B2C: Collecting Logs](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-troubleshoot-custom)

Real Me:
- [RealMe for developers](https://developers.realme.govt.nz/)
- [RealMe login service MTS](https://mts.realme.govt.nz/logon-mts/home)
