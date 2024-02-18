# Azure Active Directory B2C - RealMe Assertion service - ITE Integration

** This page needs review **

This wiki explains how to integrate Real Me Assertion service in the RealMe ITE environment:

## Certificates ordering

Purchase two certificates:
1. The first one will be used to sign the SAML requests.
2. The second one will be used to integrate with the RCMS service (client certificate authentication).

## Configuring the AAD B2C tenant

You will need to generate the metadata file before completing the ITE request.

3. Follow the integration steps as describe for the MTS environment, but you will have to use your own cert rather than using the certificate provided by RealMe (`mts_saml_sp.p12/pfx`).

## ITE Request

4. Compress saved Metadata from B2C retrieved from Assertion-MTS instructions:
(re: Download the B2C metadata file (replace `yourtenant` with the name of your B2C tenant):
  `https://yourtenant.b2clogin.com/yourtenant.onmicrosoft.com/B2C_1A_SignUpSignInRealMeAssertion/samlp/metadata?idptp=RealMeAssertion-SAML2`)

5. Create a new ITE request on the [RealMe developers website](https://developers.realme.govt.nz/projects/):
6. In ITE request, specify:
   a) Online Service integrations - Organization and project name, environment, login type (Login, Assertion or Login and Assertion) and requested integration date.
   b) SAML Service Provider configuration (for example `Azure AD B2C`) and select the zip file archive containing the Agency Metadata (extracted by the B2C SAML metadata).
   c) Complete Agency co-branding customization.
   d) Complete Service Provider SAML AuthnRequest section - AllowCreate allows users to Sign Up, and required Authentication Strength (Low, Mod or Token).
   e) Click `SAVE` and `SUBMIT TO DIA` buttons to request ITE integration.

## Obtaining the Federate Login Token

TODO Add Wiki

## Decoding identity and address claims

As part of the user journey, you can decode the safe base64 identity following this tutorial: [Decoding RealMe Claims](./Decoding-RealMe-Claims.md)

