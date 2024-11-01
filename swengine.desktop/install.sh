dotnet publish --output dist
chmod +x ./dist/swengine.desktop
chmod +x swengine
sudo cp  swengine /usr/bin
sudo mkdir -p /usr/lib/swengine && sudo cp -r dist/* /usr/lib/swengine
sudo cp swengine.desktop /usr/share/applications
echo "Installation finished"
