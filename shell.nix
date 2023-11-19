{ pkgs ? import <nixpkgs> { } }:

let
    buildLibs = with pkgs; (with xorg; [
    ]);
in with pkgs; mkShell {
    buildInputs = [
        dotnet-sdk
    ];
    shellHook = ''
        export LD_LIBRARY_PATH="$LD_LIBRARY_PATH:${lib.makeLibraryPath buildLibs}"
        export DOTNET_ROOT="${pkgs.dotnet-sdk}"
        export PATH="$PATH:$HOME/.dotnet/tools"
        dotnet tool install --global csharp-ls --version 0.5.0
    '';
}
