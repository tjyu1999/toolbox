import os
import sys
import requests as req
import zipfile

def dnld(url, fname):
    r = req.get(url, stream=True)
    size = int(r.headers.get('content-length', 0))
    curr_size = 0
    bar_length = 150

    with open(fname, 'wb') as file:
        print("Progress: [  0%] []", end="\r")
        
        for data in r.iter_content(chunk_size=1024):
            file.write(data)
            curr_size += len(data)
            
            pctg = (curr_size / size) * 100
            bar = '#' * int(pctg / (100 / bar_length)) + '.' * (bar_length - int(pctg / (100 / bar_length)))
            print(f"Progress: [{pctg:3.0f}%] [{bar}]", end="\r")
            
    print("\n")

def extr(fname, dst):
    if not os.path.exists(dst):
        os.mkdir(dst)
    
    with zipfile.ZipFile(fname, 'r') as ref:
        ref.extractall(dst)

if __name__ == "__main__":
    if len(sys.argv) < 3:
        print("The command should be \"python3 pyver.py {URL} {FILENAME}\"")
        sys.exit(1)
    
    url = sys.argv[1]
    fname = sys.argv[2] + ".zip"
    dst = sys.argv[2]
    dnld(url, fname)
    extr(fname, dst)
  
    print("Download and extraction are completed.")
