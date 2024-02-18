### Azure Active Directory B2C - RealMe Login service - ITE Integration

This wiki explains how to integrate Real Me Login in the RealMe ITE environment:

1. Purchase a certificate: it will be used to sign the SAML requests.

2. Download the [ITE login service IdP metadata zip file](https://developers.realme.govt.nz/assets/ITE-Login-service-bundle-2023.zip). It contains the ITE SAML Metadata (`B2C_1A_DIA_RealMe_LoginService.xml`)

3. Follow the integration steps as describe for the MTS environment:
- Use your own cert rather than the certificate provided by RealMe (`mts_saml_sp.pfx`).

4. Create An ITE Integration Request on the [RealMe developers website](https://developers.realme.govt.nz/)

## ITE Request

5. Compress saved Metadata from B2C retrieved from Assertion-MTS instructions:
(re: Download the B2C metadata file (replace `yourtenant` with the name of your B2C tenant):
  `https://yourtenant.b2clogin.com/yourtenant.onmicrosoft.com/B2C_1A_SignUpSignInRealMeAssertion/samlp/metadata?idptp=RealMeAssertion-SAML2`)

6. Create a new ITE request on the [RealMe developers website](https://developers.realme.govt.nz/projects/):
7. In ITE request, specify:
   a) Online Service integrations - Organization and project name, environment, login type (Pick `Login`) and requested integration date.
   b) SAML Service Provider configuration (for example `Azure AD B2C`) and select the zip file archive containing the Agency Metadata (extracted by the B2C SAML metadata).
   c) Complete Agency co-branding customization.
   d) Complete Service Provider SAML AuthnRequest section - AllowCreate allows users to Sign Up, and required Authentication Strength (Low, Mod or Token).
   e) Click `SAVE` and `SUBMIT TO DIA` buttons to request ITE integration.

