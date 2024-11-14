#!/bin/bash
APP_NAME="TestGenerator.app"
ENTITLEMENTS="TestGeneratorEntitlements.entitlements"
# SIGNING_IDENTITY="Developer ID: MyCompanyName" # matches Keychain Access certificate name
SIGNING_IDENTITY="-"

find "$APP_NAME/Contents/MacOS/"|while read fname; do
    if [[ -f $fname ]]; then
        echo "[INFO] Signing $fname"
        codesign --force --timestamp --options=runtime --entitlements "$ENTITLEMENTS" --sign "$SIGNING_IDENTITY" "$fname"
    fi
done

echo "[INFO] Signing app file"

codesign --force --timestamp --options=runtime --entitlements "$ENTITLEMENTS" --sign "$SIGNING_IDENTITY" "$APP_NAME"
