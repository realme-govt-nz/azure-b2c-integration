# Real Me (MTS) Assertion Service - Azure AD B2C integration

This wiki shows you how to enable sign-in for a RealMe user account by using custom policies in Azure Active Directory (Azure AD) B2C. You enable sign-in by adding a SAML technical profile to a custom policy.

In this tutorial:
- RealMe is treated as an external/social identity provider (IdP). 
- RealMe is integrated as any other external IdP. To configure more complex user journey, please refer to the [Useful links section](#-Useful-links)

## Prerequisites.

- Complete the steps in [Get started with custom policies in Azure Active Directory B2C](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-get-started-custom).

## Create a policy key.

You need to get the certificate used between Azure AD B2C and Relame to exchange data.

- Download the [Integration-Bundle-MTS-V2.1.zip](https://developers.realme.govt.nz/assets/Uploads/Integration-Bundle-MTS-V2.1.zip) from the [RealMe Developer Website](https://developers.realme.govt.nz/try-it-out-now/) and unzip it.
- Rename the file `mts_mutual_ssl_sp.p12` to `mts_mutual_ssl_sp.pfx`.

You need to store this certificate in your Azure AD B2C tenant.

1. Sign in to the [Azure portal](https://portal.azure.com/).
2. Make sure you're using the directory that contains your Azure AD B2C tenant by clicking the Directory and subscription filter in the top menu and choosing the directory that contains your tenant.
3. Choose **All services** in the top-left corner of the Azure portal, and then search for and select Azure AD B2C.
4. On the Overview page, select **Identity Experience Framework - PREVIEW**.
5. Select **Policy Keys** and then select **Add**.
6. For **Options**, choose `Upload`.
7. Enter this **Name** for the policy key: `SamlMessageSigning`. The prefix `B2C_1A_` might be added automatically.
8. Browse to and select the `mts_mutual_ssl_sp.pfx` file.
9. In **Password**, enter the password of the certificate (you can find this information in the `readme.txt` file in the `Integration-Bundle-MTS-V2.1.zip` zipped file)
10. Click **Create**.

## Add a claims provider.

If you want users to sign in using a RealMe account, you need to define the account as a claims provider that Azure AD B2C can communicate with through an endpoint. The endpoint provides a set of claims that are used by Azure AD B2C to verify that a specific user has authenticated.

You can define a RealMe account as a claims provider by adding it to the ClaimsProviders element in the extension file of your policy.

1. Open the *TrustFrameworkExtensions.xml* file.
2. Find the **BuildingBlocks** element. If it does not exist, add it under the root element.
3. Find the **ClaimsSchema** element inside the **BuildingBlocks** element. If it does not exist, add it under the root element **BuildingBlocks**.
4. Add new **ClaimType** elements as follows:
    ```xml
    <ClaimType Id="fit">
      <DisplayName>Federate Identity Token</DisplayName>
      <DataType>string</DataType>
      <UserHelpText />
      <UserInputType>TextBox</UserInputType>
    </ClaimType>
    <ClaimType Id="safeB64Identity">
      <DisplayName>Safe Base 64 Encoded Identity</DisplayName>
      <DataType>string</DataType>
      <UserHelpText />
      <UserInputType>TextBox</UserInputType>
    </ClaimType>
    <ClaimType Id="safeB64Address">
      <DisplayName>Safe Base 64 Encoded Address</DisplayName>
      <DataType>string</DataType>
      <UserHelpText />
      <UserInputType>TextBox</UserInputType>
    </ClaimType>
    <ClaimType Id="rcmsOpaqueToken">
      <DisplayName>The RCMS opaque token</DisplayName>
      <DataType>string</DataType>
      <UserHelpText />
      <UserInputType>TextBox</UserInputType>
    </ClaimType>
    ```
5. Find the **ClaimsProviders** element. If it does not exist, add it under the root element.
6. Add a new **ClaimsProvider** as follows:
    ```xml
    <ClaimsProvider>
      <Domain>realme.govt.nz</Domain>
      <DisplayName>Real Me Assertion</DisplayName>
      <TechnicalProfiles>
        <TechnicalProfile Id="RealMeAssertion-SAML2">
          <DisplayName>Real Me Assertion</DisplayName>
          <Description>Login and verify your identity with your RealMe account</Description>
          <Protocol Name="SAML2" />
          <Metadata>
            <Item Key="IssuerUri">yourEntityID</Item>
            <Item Key="WantsEncryptedAssertions">true</Item>
            <Item Key="WantsSignedAssertions">true</Item>
            <Item Key="WantsSignedRequests">true</Item>
            <Item Key="ResponsesSigned">false</Item>
            <Item Key="NameIdPolicyAllowCreate">true</Item>
            <Item Key="NameIdPolicyFormat">urn:oasis:names:tc:SAML:2.0:nameid-format:transient</Item>
            <Item Key="IncludeAuthnContextClassReferences">urn:nzl:govt:ict:stds:authn:deployment:GLS:SAML:2.0:ac:classes:ModStrength</Item>
            <Item Key="PartnerEntity"><![CDATA[
    <EntityDescriptor entityID="https://mts.realme.govt.nz/realmemts/realmeidp" xmlns="urn:oasis:names:tc:SAML:2.0:metadata">
    <IDPSSODescriptor WantAuthnRequestsSigned="true" protocolSupportEnumeration="urn:oasis:names:tc:SAML:2.0:protocol">
    <KeyDescriptor use="signing">
    <ds:KeyInfo xmlns:ds="http://www.w3.org/2000/09/xmldsig#">
    <ds:X509Data>
    <ds:X509Certificate>
    MIIEDTCCAvWgAwIBAgIEfNnfQTANBgkqhkiG9w0BAQUFADCBtjELMAkGA1UEBhMCTloxEzARBgNV
    BAgTCldlbGxpbmd0b24xEzARBgNVBAcTCldlbGxpbmd0b24xJzAlBgNVBAoTHkRlcGFydG1lbnQg
    b2YgSW50ZXJuYWwgQWZmYWlyczEnMCUGA1UECxMeRGVwYXJ0bWVudCBvZiBJbnRlcm5hbCBBZmZh
    aXJzMSswKQYDVQQDEyJtdHMuc2lnbmluZy5hY2NvdW50LnJlYWxtZS5nb3Z0Lm56MB4XDTEzMDkx
    MTIxNTkxOFoXDTIzMDkwOTIxNTkxOFowgbYxCzAJBgNVBAYTAk5aMRMwEQYDVQQIEwpXZWxsaW5n
    dG9uMRMwEQYDVQQHEwpXZWxsaW5ndG9uMScwJQYDVQQKEx5EZXBhcnRtZW50IG9mIEludGVybmFs
    IEFmZmFpcnMxJzAlBgNVBAsTHkRlcGFydG1lbnQgb2YgSW50ZXJuYWwgQWZmYWlyczErMCkGA1UE
    AxMibXRzLnNpZ25pbmcuYWNjb3VudC5yZWFsbWUuZ292dC5uejCCASIwDQYJKoZIhvcNAQEBBQAD
    ggEPADCCAQoCggEBAL5spWvmZQJKBSQmwNI/BvAqdP7QSQnTbcKeGuI9uknY2BQPmDWV8D+6ojuJ
    ntyKvP+SKDK1eHE89k3Sb6dz6valnyQ4AIT8wEVt/Q0OZRWjng1OQw+Fo2Ica7HmEcBrlyGlMmUX
    4ncibpjvDAxLs3FhcF+nn+9LSa/nz45kkonNbesk7ovlwupfCj1EySYbEPfOsljocbfc7H6QVl+N
    FucezIDPWFB/M+oxijXidDcDrACS13H7kS6NcYccBFeHmL/P6hLTeBSjI9p5TzCyMiRZmGCPvJ+I
    1IuX6AL68gC9xWW/JmX8CCig4z7xKwO4hh+nfqNVmAKz4gUgqiK4EUsCAwEAAaMhMB8wHQYDVR0O
    BBYEFGThS3UfJlaQ/+AVOO+bNnUhm+dBMA0GCSqGSIb3DQEBBQUAA4IBAQCTEhfswHg0VbAQg+kw
    U1Ri/hvLCOakyp3zujhZmSyyXA9UbCTTwv6utwy0n7h0/ZYEC5HzhZiA9DYjchOiEWg8tv3J3+43
    +xwJYXbEb4pwbWR1QKwVwNMWeLfWYyZWDlaGQYbsjyv8U928zryGcMghjXkNIzy4rEN5pA9VX6bD
    cSTH13DC87PWJcn3aiP4z++k3q6fu/5uFp2LbFRFUGIfblpr38rB3CfVXRZ0hyXIhCNy4tnlI7Mu
    DZ9vwQN0dNet0cUat6wK7GHWA4oDa6aJc/BlHZ5IOYEdopNYwWbFTXmzxHWqwXQ+URku0YrxcIT5
    Rv0MLYj/pxWGJ6PZjRbd
                        </ds:X509Certificate>
    </ds:X509Data>
    </ds:KeyInfo>
    </KeyDescriptor>
    <ArtifactResolutionService index="0" isDefault="true" Binding="urn:oasis:names:tc:SAML:2.0:bindings:SOAP" Location="https://as.mts.realme.govt.nz/sso/ArtifactResolver/metaAlias/realmemts/realmeidp"/>
    <NameIDFormat>urn:oasis:names:tc:SAML:2.0:nameid-format:transient</NameIDFormat>
    <NameIDFormat>urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified</NameIDFormat>
    <SingleSignOnService Binding="urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect" Location="https://mts.realme.govt.nz/realme-mts/validate/realme-mts-idp.xhtml"/>
    </IDPSSODescriptor>
    </EntityDescriptor>]]></Item>
          </Metadata>
          <CryptographicKeys>
            <Key Id="SamlMessageSigning" StorageReferenceId="B2C_1A_SamlMessageSigning" />
            <Key Id="SamlAssertionDecryption" StorageReferenceId="B2C_1A_SamlMessageSigning" />
          </CryptographicKeys>
          <OutputClaims>
            <OutputClaim ClaimTypeReferenceId="issuerUserId" PartnerClaimType="yourEntityID" />
            <OutputClaim ClaimTypeReferenceId="fit" PartnerClaimType="urn:nzl:govt:ict:stds:authn:attribute:igovt:IVS:FIT" />
            <OutputClaim ClaimTypeReferenceId="safeB64Identity" PartnerClaimType="urn:nzl:govt:ict:stds:authn:safeb64:attribute:igovt:IVS:Assertion:Identity" />
            <OutputClaim ClaimTypeReferenceId="safeB64Address" PartnerClaimType="urn:nzl:govt:ict:stds:authn:safeb64:attribute:NZPost:AVS:Assertion:Address" />
            <OutputClaim ClaimTypeReferenceId="rcmsOpaqueToken" PartnerClaimType="urn:nzl:govt:ict:stds:authn:attribute:rcms_opaque_token" />
            <OutputClaim ClaimTypeReferenceId="identityProvider" DefaultValue="realme.govt.nz" AlwaysUseDefaultValue="true" />
            <OutputClaim ClaimTypeReferenceId="authenticationSource" DefaultValue="socialIdpAuthentication" AlwaysUseDefaultValue="true" />
          </OutputClaims>
          <OutputClaimsTransformations>
            <OutputClaimsTransformation ReferenceId="CreateRandomUPNUserName" />
            <OutputClaimsTransformation ReferenceId="CreateUserPrincipalName" />
            <OutputClaimsTransformation ReferenceId="CreateAlternativeSecurityId" />
          </OutputClaimsTransformations>
          <UseTechnicalProfileForSessionManagement ReferenceId="SM-Noop" />
        </TechnicalProfile>
      </TechnicalProfiles>
    </ClaimsProvider>
    ```

4. Replace `yourEntityID` with a valid RealMe Issuer (see [RealMe request parameters](https://developers.realme.govt.nz/how-realme-works/realme-request-parameters/)) in this format `https://www.agencyname.govt.nz/context/application-name`
5. **Old versions of the custom policies use `socialIdpUserId` rather than `issuerUserId`**.
6. Save the file.  

## Upload the extension file for verification

By now, you have configured your policy so that Azure AD B2C knows how to communicate with RealMe account. Try uploading the extension file of your policy just to confirm that it doesn't have any issues so far.

1. On the **Custom Policies** page in your Azure AD B2C tenant, select **Upload Policy**.
2. Enable **Overwrite the policy if it exists**, and then browse to and select the *TrustFrameworkExtensions.xml* file.
3. Click **Upload**.

## Register the claims provider

At this point, the identity provider has been set up, but it’s not available in any of the sign-up or sign-in screens. To make it available, you create a duplicate of an existing template user journey, and then modify it so that it also has the RealMe identity provider.

1. Open the *TrustFrameworkBase.xml* file from the starter pack.
2. Find and copy the entire contents of the **UserJourney** element that includes `Id="SignUpOrSignIn"`.
3. Open the *TrustFrameworkExtensions.xml* and find the **UserJourneys** element. If the element doesn't exist, add one.
4. Paste the entire content of the **UserJourney** element that you copied as a child of the **UserJourneys** element.
5. Rename the ID of the user journey. For example, `SignUpSignInRealMeAssertion`.

### Display the button

The **ClaimsProviderSelection** element is analogous to an identity provider button on a sign-up or sign-in screen. If you add a **ClaimsProviderSelection** element for a RealMe account, a new button shows up when a user lands on the page.

1. Find the **OrchestrationStep** element that includes `Order="1"` in the user journey that you created.
2. Under **ClaimsProviderSelections**, add the following element. Set the value of **TargetClaimsExchangeId** to an appropriate value, for example `RealMeExchange`:
    ```xml
    <ClaimsProviderSelection TargetClaimsExchangeId="RealMeExchange" />
    ```

### Link the button to an action

Now that you have a button in place, you need to link it to an action. The action, in this case, is for Azure AD B2C to communicate with a RealMe account to receive a token.

1. Find the **OrchestrationStep** that includes `Order="2"` in the user journey.
2. Add the following **ClaimsExchange** element making sure that you use the same value for **Id** that you used for **TargetClaimsExchangeId**:
    ```xml
    <ClaimsExchange Id="RealMeExchange" TechnicalProfileReferenceId="RealMeAssertion-SAML2" />
    ```

    Update the value of **TechnicalProfileReferenceId** to the **Id** of the technical profile you created earlier. For example, `RealMeAssertion-SAML2`.

3. Save the *TrustFrameworkExtensions.xml* file and upload it again for verification.

### Update the relying party file

Update the relying party (RP) file that initiates the user journey that you created.

1. Make a copy of *SignUpOrSignIn.xml* in your working directory, and rename it. For example, rename it to SignUpSignInRealMeAssertion.xml.
2. Open the new file and update the value of the **PolicyId** attribute for **TrustFrameworkPolicy** with a unique value. For example, `SignUpSignInRealMeAssertion`.
3. Update the value of **PublicPolicyUri** with the URI for the policy. For example,`http://yourtenant.onmicrosoft.com/B2C_1A_SignUpSignInRealMeAssertion`.
4. Update the value of the **ReferenceId** attribute in **DefaultUserJourney** to match the ID of the new user journey that you created (SignUpSignInRealMeAssertion).
5. Add the claims returned by RealMe inside the **OutputClaims** element:
    ```xml
    <OutputClaim ClaimTypeReferenceId="issuerUserId" />
    <OutputClaim ClaimTypeReferenceId="identityProvider" />
    <OutputClaim ClaimTypeReferenceId="fit" />
    <OutputClaim ClaimTypeReferenceId="safeB64Identity" />
    <OutputClaim ClaimTypeReferenceId="safeB64Address" />
    <OutputClaim ClaimTypeReferenceId="rcmsOpaqueToken" />
    ```
6. Save your changes, upload the file.

### Configure a RealMe relying party trust

To use RealMe as an identity provider in Azure AD B2C, you need to create a RealMe Relying Party Trust with the Azure AD B2C SAML metadata. The following example shows a URL address to the SAML metadata of an Azure AD B2C technical profile:

```
https://login.microsoftonline.com/te/yourtenant.onmicrosoft.com/B2C_1A_SignUpSignInRealMeAssertion/samlp/metadata?idptp=RealMeAssertion-SAML2
https://yourtenant.b2clogin.com/yourtenant.onmicrosoft.com/B2C_1A_SignUpSignInRealMeAssertion/samlp/metadata?idptp=RealMeAssertion-SAML2
```

1. Replace the following value **yourtenant** with your tenant name, such as yourtenant.onmicrosoft.com.
2. Open a browser and navigate to the URL. Make sure you type the correct URL and that you have access to the XML metadata file.
3. Save the *metadata.xml* file.
4. Open the file and remove the `<Signature>...</Signature>` tag.
5. Browse this url: https://mts.realme.govt.nz/realme-mts/metadata/import.xhtml
6. Select the metadata file you want to upload then click **Validate**.
7. If the file is valid, you can click **Import**.
8. On the next page, click **Import** then **Continue**.
9. Update your configuration: https://mts.realme.govt.nz/realme-mts/metadata/updateconfiguration.xhtml
10. Select `yourEntityID` in the **entity ID** field.
11. Select `JWT Opaque Token` in the **Opaque Token Return Type** dropdown.
12. Select the fields you need to be returned.
13. Click **Submit**.

## Create an Azure AD B2C application

Communication with Azure AD B2c occurs through an application that you create in your tenant. This section lists optional steps you can complete to create a test application if you haven't already done so.

1. Sign in to the [Azure portal](https://portal.azure.com/).
2. Make sure you're using the directory that contains your Azure AD B2C tenant by clicking the **Directory and subscription filter** in the top menu and choosing the directory that contains your tenant.
3. Choose **All services** in the top-left corner of the Azure portal, and then search for and select **Azure AD B2C**.
4. Select **Applications**, and then select **Add**.
5. Enter a name for the application, for example *jwt.ms*.
6. For **Web App / Web API**, select `Yes`, and then enter `https://jwt.ms` for the **Reply URL**.

### Test the relying party file

5. Select the policy created previously (`B2C_1A_SignUpSignInRealMeAssertion`).
6. Make sure that Azure AD B2C application that you created is selected in the **Select application** field, and then test it by clicking **Run now**.
9. On the RealMe website, fill the IVs attributes then click on `Initiate SAML Response`, it will redirect you to the https://jwt.ms/ website.

You can inspect the token returned by B2C:
- The **idp** claim contains the B2C `realme.govt.nz`.
- The **safeB64Identity** claim contains the RealMe `Verified Identity`.
- The **safeB64Address** claim contains the RealMe `Verified Address`.
- The **rcmsOpaqueToken** claim contains the RealMe `RCMS opaque token`. 
- The **issuerUserId** and **fit** claims will be returned correctly once integrated in the RealMe ITE environment.

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
