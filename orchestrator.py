import subprocess, os
root = '/home/joeblack/Documents/Deepiri/deepiri-vox'
scripts = ['sensor.py', 'brain.py', 'size.py', 'git.py', 'health.py', 'stats.py', 'filetypes.py', 'readme.py', 'help.py']

print("╔═══════════════════════════════════════════════════════════════════╗")
print("║         🚀 DEEPIRI VOX - PRINT-ONLY ORCHESTRATOR                 ║")
print("╚═══════════════════════════════════════════════════════════════════╝")
print()

for script in scripts:
    print(f"▶ RUNNING: {script}")
    p = subprocess.run(['python3', f'{root}/{script}'], capture_output=True, text=True, cwd=root)
    print(p.stdout)

print("═══════════════════════════════════════════════════════════════════")
print("✓ DEEPIRI VOX TRANSMISSION COMPLETE")
print("═══════════════════════════════════════════════════════════════════")