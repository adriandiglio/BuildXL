import {Cmd, Artifact, Transformer} from "Sdk.Transformers";
import {Clang, Xcode} from "Sdk.MacOS";

namespace Sandbox {
    export declare const qualifier : {
        configuration: "debug" | "release"
    };

    interface Args {
        project: StaticDirectory,
        scheme: string,
        configuration?: string,
        outFiles?: PathFragment[],
        semaphores?: string[],
        xcconfig?: File,
        dependencies?: StaticDirectory[],
        overrideXcodeBuildPath?: File,
        headerSearchPaths?: Xcode.HeaderSearchPath[];
    }

    interface Result {
        outFiles: DerivedFile[],
        derivedDataOutDir: StaticDirectory
    }

    const sourceFileDependencies = (() =>
    {
        const sandboxDir = d`${Context.getMount("Sandbox").path}`;
        const thirdPartyDir = d`../../../../third_party`;

        const sourceSealOptions =  Transformer.SealSourceDirectoryOption.allDirectories;
        const globPattern = "*";

        return BuildXLSdk.Flags.isMicrosoftInternal ?
            [
                Transformer.sealSourceDirectory(sandboxDir, sourceSealOptions),
                Transformer.sealSourceDirectory(thirdPartyDir, sourceSealOptions)
            ] : [
                ...Transformer.sealDirectory(sandboxDir, globR(sandboxDir, globPattern)).contents,
                ...Transformer.sealDirectory(thirdPartyDir, globR(thirdPartyDir, globPattern)).contents
            ];
    })();

    export function build(args: Args): Result {
        const conf = args.configuration || qualifier.configuration;
        const outDir = Context.getNewOutputDirectory(args.scheme);
        const outFilePaths = (args.outFiles || []).map(a => p`${outDir}/Build/Products/${conf}/${a}`);
        const result = Xcode.execute({
            useModernBuildSystem: true,
            project: args.project,
            xcconfig: args.xcconfig,
            scheme: args.scheme,
            semaphores: args.semaphores,
            configuration: conf,
            derivedDataPath: outDir,
            declaredOutputs: [
                ...outFilePaths,
            ],
            dependencies: [
                ...(args.dependencies || []),
                ...sourceFileDependencies
            ],
            overrideXcodeBuildPath: args.overrideXcodeBuildPath,
            headerSearchPaths: args.headerSearchPaths
        });

        return {
            outFiles: outFilePaths.map(result.getOutputFile),
            derivedDataOutDir: result.getOutputDirectory(outDir)
        };
    }

    const bundleInfoXCConfig = qualifier.configuration === "debug"
        ? f`BundleInfoDebug.xcconfig`
        : f`BundleInfo.xcconfig`;

    const isMacOs = Context.getCurrentHost().os === "macOS";

    const bxlAppXcodeproj = Transformer.sealDirectory({
        root: d`App/BuildXL.xcodeproj`,
        files: globR(d`App/BuildXL.xcodeproj`, "*")
    });

    const detoursXcodeproj = Transformer.sealDirectory({
        root: d`Detours/Detours.xcodeproj`,
        files: globR(d`Detours/Detours.xcodeproj`, "*")
    });

    const interopXcodeproj = Transformer.sealDirectory({
        root: d`Interop/Interop.xcodeproj`,
        files: globR(d`Interop/Interop.xcodeproj`, "*")
    });

    const sandboxXcodeproj = Transformer.sealDirectory({
        root: d`Sandbox/Sandbox.xcodeproj`,
        files: globR(d`Sandbox/Sandbox.xcodeproj`, "*")
    });

    @@public
    export const bxlESDaemon = isMacOs && build({
        project: bxlAppXcodeproj,
        scheme: "BuildXLSandboxDaemon",
        outFiles: [
            a`BuildXLSandboxDaemon`,
            a`com.microsoft.buildxl.sandbox.plist`
        ],
        xcconfig: bundleInfoXCConfig
    }).outFiles;

    @@public
    export const libInterop = isMacOs && build({
        project: interopXcodeproj,
        scheme: "InteropLibrary",
        outFiles: [ a`libBuildXLInterop.dylib` ],
        xcconfig: bundleInfoXCConfig
    }).outFiles[0];

    @@public
    export const libDetours = isMacOs && build({
        project: detoursXcodeproj,
        scheme: "DetoursLibrary",
        outFiles: [ a`libBuildXLDetours.dylib` ],
        xcconfig: bundleInfoXCConfig
    }).outFiles[0];

    @@public
    export const coreDumpTester = isMacOs && build({
        project: interopXcodeproj,
        scheme: "CoreDumpTester",
        outFiles: [ a`CoreDumpTester` ]
    }).outFiles[0];

    @@public
    export const monitor = isMacOs && build({
        project: sandboxXcodeproj,
        scheme: "SandboxMonitor",
        outFiles: [ a`SandboxMonitor` ],
        xcconfig: bundleInfoXCConfig
    }).outFiles[0];

    interface KextFiles {
        plist: DerivedFile,
        sandbox: DerivedFile,
        license: DerivedFile,
        codeRes: DerivedFile,
        dSYMPlist: DerivedFile,
        dSYMDwarf: DerivedFile
    }

    function buildKext(bundleInfo: File): KextFiles {
        const kernelHeaders = <Xcode.HeaderSearchPath>{
            type: "system",
            directory: importFrom("Apple.Darwin.Xnu").extracted,
            recursive: true
        };

        const result = build({
            project: sandboxXcodeproj,
            scheme: "BuildXLSandbox",
            xcconfig: bundleInfo || bundleInfoXCConfig,
            headerSearchPaths: [ kernelHeaders ],
            outFiles: [
                r`BuildXLSandbox.kext/Contents/Info.plist`,
                r`BuildXLSandbox.kext/Contents/MacOS/BuildXLSandbox`,
                r`BuildXLSandbox.kext/Contents/Resources/LICENSE`,
                r`BuildXLSandbox.kext/Contents/_CodeSignature/CodeResources`,
                ...addIfLazy(qualifier.configuration === "release", () => [
                    r`BuildXLSandbox.kext.dSYM/Contents/Info.plist`,
                    r`BuildXLSandbox.kext.dSYM/Contents/Resources/DWARF/BuildXLSandbox`
                ])
            ],
            dependencies: [
                kernelHeaders.directory
            ]
        });
        return {
            plist: result.outFiles[0],
            sandbox: result.outFiles[1],
            license: result.outFiles[2],
            codeRes: result.outFiles[3],
            dSYMPlist: qualifier.configuration === "release" ? result.outFiles[4] : undefined,
            dSYMDwarf: qualifier.configuration === "release" ? result.outFiles[5] : undefined,
        };
    }

    @@public
    export const kext = isMacOs && buildKext(bundleInfoXCConfig);
}
