# Real Me (MTS) Assertion Service - Azure AD B2C integration

This projects explains how to configure Real Me as a SAML identity provider into Azure AD B2C. It uses Azure AD B2C custom policies.

In this tutorial:
- RealMe is treated as an external identity provider (IdP).
- The only configured IdP is RealMe and we don't ask user to provide any futher information so from a user perspective there is no interaction with Azure AD B2C. To configure more complex user journey, please refer to the [Useful links section](#Useful-links)

### 1. Creating an Azure AD B2C Tenant

Follow [this tutorial](https://docs.microsoft.com/en-us/azure/active-directory-b2c/tutorial-create-tenant) to:

- Create a new Azure AD B2C tenant
- Link your Azure AD B2C tenant to a subscription

### 2. Creating signing and encryption keys.

Follow [this tutorial](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-get-started-custom#add-signing-and-encryption-keys) to:

- Create a signing key used to sign the id_token return by Azure AD B2C
- Create an encryption key used to encrypt the refresh_token return by Azure AD B2C

Next step is to upload the certificate used between Azure AD B2C and Relame to exchange data.

- Download the [Integration-Bundle-MTS-V2.1.zip](https://developers.realme.govt.nz/assets/Uploads/Integration-Bundle-MTS-V2.1.zip) from the [RealMe Developer Website](https://developers.realme.govt.nz/try-it-out-now/) and unzip it.
- Rename the file `mts_mutual_ssl_sp.p12` to `mts_mutual_ssl_sp.pfx`.
- Upload the certificate:
  1. Select **Policy Keys** and then select **Add**.
  2. For **Options**, choose `Upload`.
  3. In **Name**, enter `SamlMessageSigning`. The prefix B2C_1A_ might be added automatically.
  4. In **File upload**, select the `mts_mutual_ssl_sp.pfx` file.
  5. In **Password**, enter the password of the certificate (you can find this information in the `readme.txt` file in the `Integration-Bundle-MTS-V2.1.zip` zipped file)
  6. Click **Create**.

### 3. Customizing the Custom policies files.

You can find a custom policy started pack here:
- [Azure-Samples/active-directory-b2c-custom-policy-starterpack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack)

The policies files used in this tutorial have been modified from the [SocialAndLocalAccounts](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccounts) starter pack.

To know more about policies files, you can read the associated documentation:
- [Policy files](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-overview-custom#policy-files)

1. Download these files:
- [TrustFrameworkBase.xml](./RealMe-Assertion-MTS/TrustFrameworkBase.xml)
- [TrustFrameworkExtensions.xml](./RealMe-Assertion-MTS/TrustFrameworkExtensions.xml)
- [SignUpSignInRealMeAssertion.xml](./RealMe-Assertion-MTS/SignUpSignInRealMeAssertion.xml)

2. In these files, replace these parameters and save the files.:
- `yourtenant` with the name of your B2C tenant (without the `.onmicrosoft.com`)
- `yourEntityID` with a valid RealMe Issuer (see [RealMe request parameters](https://developers.realme.govt.nz/how-realme-works/realme-request-parameters)) in this format `https://www.agencyname.govt.nz/context/application-name`

3. Upload the policies:
- On the Custom Policies page of Identity Experience Framework, select **Upload Policy**.
- In this order, upload `TrustFrameworkBase.xml`, `TrustFrameworkExtensions.xml`, `SignUpSignInRealMeAssertion.xml`.

### 4. Upload the B2C metadata file to RealMe

1. Download the B2C metadata file (replace `yourtenant` with the name of your B2C tenant):
  `https://login.microsoftonline.com/te/yourtenant.onmicrosoft.com/B2C_1A_SignUpSignInRealMeAssertion/samlp/metadata?idptp=RealMeAssertion-SAML2`
- If you want to use the `b2clogin.com` domain, download the metadata file from this url (replace `yourtenant` with the name of your B2C tenant):
  `https://yourtenant.b2clogin.com/yourtenant.onmicrosoft.com/B2C_1A_SignUpSignInRealMeAssertion/samlp/metadata?idptp=RealMeAssertion-SAML2`

2. Open the file and remove the `<Signature>...</Signature>` tag.

3. Browse this url: https://mts.realme.govt.nz/realme-mts/metadata/import.xhtml
- Select the metadata file you want to upload then click **Validate**.
- If the file is valid, you can click **Import**.
- On the next page, click **Import** then **Continue**.
- Update your configuration: https://mts.realme.govt.nz/realme-mts/metadata/updateconfiguration.xhtml
- Select `yourEntityID` in the **entity ID** field.
- Select `JWT Opaque Token` in the **Opaque Token Return Type** dropdown.
- Select the fields you need to be returned.
- Click **Submit**.

### 5. Testing the policy

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

5. On the  **Indentiy Experience Framework**, select the `B2C_1A_SignUpSignInRealMeAssertion` policy:
6. The previously created application should be preselected otherwithe select `jwt.ms` in the **Select application** dropdown.
7. Select the domain you want to use. This should be the based on the metadata file you've uploaded to realme (see [step 1](#-4.-Upload-the-B2C-metadata-file-to-RealMe))
8. Click on the **Run now** button, you will be redirected to RealMe
9. On the RealMe website, fill the IVs attributes then click on `Initiate SAML Response`, it will redirect you to the https://jwt.ms/ website.

You can inspect the token returned by B2C:
- The **sub** claim contains the B2C `objectid`.
- The **idp** claim contains the B2C `realme.govt.nz`.
- The **safeB64Identity** claim contains the RealMe `Verified Identity`.
- The **safeB64Address** claim contains the RealMe `Verified Address`.
- The **rcmsOpaqueToken** claim contains the RealMe `RCMS opaque token`. 
- The **issuerUserId** and **fit** claims will be returned correctly once integrated in the RealMe ITE environment.

### 6. Decoding the verified identity and address

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

### Useful links

Azure Active Directory B2C:
- [Azure Active Directory B2C Overview](https://azure.microsoft.com/en-us/services/active-directory-b2c/)
- [Custom policies in Azure Active Directory B2C](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-overview-custom)
- [Define a SAML technical profile in an Azure Active Directory B2C custom policy](https://docs.microsoft.com/en-us/azure/active-directory-b2c/saml-technical-profile)
- [Azure Active Directory B2C: Collecting Logs](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-troubleshoot-custom)

Real Me:
- [RealMe for developers](https://developers.realme.govt.nz/)
- [RealMe assertion service MTS](https://mts.realme.govt.nz/realme-mts/home/information.xhtml)
