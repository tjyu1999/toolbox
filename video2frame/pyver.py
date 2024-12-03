import os
import sys
from datetime import datetime
import yt_dlp
from moviepy import VideoFileClip
from PIL import Image
from tqdm import tqdm

def download_video(url, q, vdir):
    ydl_opts = {
        'format': f'bestvideo[height <= {q}] + bestaudio/best[height <= {q}]',
        'merge_output_format': 'mp4',
        'outtmpl': os.path.join(vdir, f'{vdir}.mp4')
    }
    
    with yt_dlp.YoutubeDL(ydl_opts) as ydl:
        ydl.download([url])
            
    return os.path.join(vdir, f'{vdir}.mp4')

def extract_frames(vdir, fdir):
    clip = VideoFileClip(vdir)
    frame_count = int(clip.fps * clip.duration)
        
    with tqdm(total=frame_count, desc="[clip] Extracting frames", unit="frame") as pbar:
        for i in range(frame_count):
            frame = clip.get_frame(i / clip.fps)
            frame_fname = os.path.join(fdir, f"frame_{i:06d}.png")
            frame = Image.fromarray(frame)
            frame.save(frame_fname)
            pbar.update(1)
    
        clip.close()

def main():
    if len(sys.argv) < 2:
        sys.exit(1)
    
    url = sys.argv[1]
    q = sys.argv[2] if len(sys.argv) > 2 else '720p'
    
    vdir = datetime.now().strftime("%d%H%M%S")
    if not os.path.exists(vdir):
        os.makedirs(vdir)
    
    fdir = os.path.join(vdir, "frames")
    if not os.path.exists(fdir):
        os.makedirs(fdir)
    
    vdir = download_video(url, q, vdir)
    extract_frames(vdir, fdir)

if __name__ == "__main__":
    main()
